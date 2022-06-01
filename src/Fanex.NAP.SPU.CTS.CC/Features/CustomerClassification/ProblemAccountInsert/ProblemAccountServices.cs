namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountServices
    {
        IEnumerable<ExistedProblemAccountEntity> GetExistedProblemAccounts(IEnumerable<long> custIds);

        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId);

        InsertPAResultEntity InsertProblemAccountLogics(IEnumerable<InsertProblemAccountEntity> problemAccounts, FunctionId funcId, InsertPA_ByType insertByType, bool isFromQueue, string serviceName);

        RemovePAResultEntity RemoveProblemAccountLogics(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName);
  
        void RemoveOldProblemAccount(IEnumerable<long> custIds);

        IEnumerable<CustInfoEntity> InsertRobotAccount(IEnumerable<InsertRobotEntity> robotAccounts);

        void InsertPALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName);

        void RemovePALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName);
    }

    public class ProblemAccountServices : IProblemAccountServices
    {
        private readonly IProblemAccountDataAccess problemAccountDataAccess;        
        private readonly ISentryClient sentryClient;

        public ProblemAccountServices(
            IProblemAccountDataAccess problemAccountDataAccess,            
            ISentryClient sentryClient)
        {
            this.problemAccountDataAccess = problemAccountDataAccess;            
            this.sentryClient = sentryClient;
        }

        public IEnumerable<ExistedProblemAccountEntity> GetExistedProblemAccounts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var custIdsToString = string.Join(",", custIds);

                return problemAccountDataAccess.GetExistedProblemAccounts(custIdsToString);
            }

            return Enumerable.Empty<ExistedProblemAccountEntity>();
        }

        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId)
        {
            if (custIds != null && custIds.Any())
            {
                var custIdsToString = string.Join(",", custIds);

                return problemAccountDataAccess.GetProblemAccountLosingClassification(custIdsToString, funcId);
            }

            return Enumerable.Empty<ProblemAccountLosingClassificationEntity>();
        }

        public InsertPAResultEntity InsertProblemAccountLogics(IEnumerable<InsertProblemAccountEntity> problemAccounts, FunctionId funcId, InsertPA_ByType insertByType, bool isFromQueue, string serviceName)
        {
            var result = new InsertPAResultEntity();

            if (problemAccounts != null && problemAccounts.Any())
            {
                var paLosing = GetProblemAccountLosingClassification(problemAccounts.Select(c => c.CustID).Distinct(), funcId);

                var paClassification = MergePALosingClassification(problemAccounts, paLosing);

                result = InsertProblemAccount(paClassification, insertByType, isFromQueue, serviceName);

                if (result.ErrorCode == 0)
                {
                    InsertPALogics(result.CustomerInfo, serviceName);
                }
            }

            return result;
        }

        public RemovePAResultEntity RemoveProblemAccountLogics(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName)
        {
            var result = new RemovePAResultEntity();

            if (problemAccounts != null && problemAccounts.Any())
            {
                result = RemoveProblemAccount(problemAccounts, isFromQueue, serviceName);

                if (result.ErrorCode == 0)
                {
                    RemovePALogics(result.CustomerInfo, serviceName);
                }
            }

            return result;
        }

        public void InsertPALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                CheckNewPAAssociationCustomerForRollback(customerInfo);

                var custIds = customerInfo.Select(x => x.CustID).Distinct();

                UpdateBetLimitOfFlaggedCustomer(custIds, serviceName);
            }
        }

        public void RemovePALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custIds = customerInfo.Select(x => x.CustID).Distinct();

                UpdateBetLimitOfUnflaggedCustomer(custIds, serviceName);
            }
        }

        public void RemoveOldProblemAccount(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                problemAccountDataAccess.RemoveOldProblemAccount(string.Join(",", custIds.Distinct()));
            }
        }

        public IEnumerable<CustInfoEntity> InsertRobotAccount(IEnumerable<InsertRobotEntity> robotAccounts)
        {
            if (robotAccounts != null && robotAccounts.Any())
            {
                var customerInfo = problemAccountDataAccess.InsertRobotAccount(JsonConvert.SerializeObject(robotAccounts));

                CheckNewPAAssociationCustomerForRollback(customerInfo);

                return customerInfo;
            }

            return Enumerable.Empty<CustInfoEntity>();
        }

        #region Private Methods

        private void CheckNewPAAssociationCustomerForRollback(IEnumerable<CustInfoEntity> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);
            }
        }

        private InsertPAResultEntity InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> problemAccounts, InsertPA_ByType insertByType, bool isFromQueue, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = problemAccountDataAccess.InsertProblemAccount(JsonConvert.SerializeObject(problemAccounts), Enum.GetName(typeof(InsertPA_ByType), insertByType), isFromQueue);

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccount", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
                }

                return result;
            }

            return new InsertPAResultEntity();
        }

        private RemovePAResultEntity RemoveProblemAccount(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = problemAccountDataAccess.RemoveProblemAccount(problemAccounts, isFromQueue);

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "RemoveProblemAccount", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
                }

                return result;
            }

            return new RemovePAResultEntity();
        }

        private bool UpdateBetLimitOfFlaggedCustomer(IEnumerable<long> custId, string serviceName)
        {
            if (custId != null && custId.Any())
            {
                var custIdsToString = string.Join(",", custId);

                var result = problemAccountDataAccess.UpdateBetLimitOfFlaggedCustomer(custIdsToString);

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "UpdateBetLimitOfFlaggedCustomer", custId, new Exception(result.ErrorMsg), sentryClient);

                    return false;
                }
            }

            return true;
        }

        private bool UpdateBetLimitOfUnflaggedCustomer(IEnumerable<long> custId, string serviceName)
        {
            if (custId != null && custId.Any())
            {
                var custIdsToString = string.Join(",", custId);

                var result = problemAccountDataAccess.UpdateBetLimitOfUnflaggedCustomer(custIdsToString);

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "UpdateBetLimitOfUnflaggedCustomer", custId, new Exception(result.ErrorMsg), sentryClient);

                    return false;
                }
            }

            return true;
        }

        private IEnumerable<InsertProblemAccountEntity> MergePALosingClassification(IEnumerable<InsertProblemAccountEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> paLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(paLosing ?? Enumerable.Empty<ProblemAccountLosingClassificationEntity>(),
                                                 pa => pa.CustID,
                                                 pl => pl.CustId,
                                                (pa, pl) => new { pa, pl })
                                                .SelectMany(
                                                        group => group.pl.DefaultIfEmpty(),
                                                        (group, temp) => new InsertProblemAccountEntity
                                                        {
                                                            CTSCustID = group.pa.CTSCustID,
                                                            CustID = group.pa.CustID,
                                                            RoleID = group.pa.RoleID,
                                                            SubscriberID = group.pa.SubscriberID,
                                                            CategoryID = group.pa.CategoryID,
                                                            CategoryGroup = group.pa.CategoryGroup,
                                                            IsLicensee = group.pa.IsLicensee,
                                                            CreatedBy = StarixITId,
                                                            Remark = group.pa.Remark,
                                                           
                                                            IsDirectPA = group.pa.IsDirectPA,
                                                            IsFromCTS = false,
                                                            IsFromTVS = false,
                                                            IsFromTW = false,
                                                            IsDataChanged = false,
                                                            ReasonID = group.pa.ReasonID,

                                                            BetCount = temp?.BetCount,
                                                            ActiveDays = temp?.ActiveDays,
                                                            TurnoverRM = temp?.TurnoverRM,
                                                            WinlossRM = temp?.WinlossRM,
                                                            WinlossStatus = temp?.WinlossStatus,
                                                           
                                                        }); ;
            }

            return Enumerable.Empty<InsertProblemAccountEntity>();
        }       
        
        #endregion
    }
}