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

        IEnumerable<long> InsertProblemAccountPackage(IEnumerable<InsertProblemAccountEntity> problemAccounts, FunctionId funcId, InsertPA_ByType insertByType, bool isToQueue, string serviceName);

        RemovePAResultEntity RemoveProblemAccountPackage(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName);

        void RemoveOldProblemAccount(IEnumerable<long> custIds);

        IEnumerable<CustInfoEntity> InsertRobotAccountPackage(IEnumerable<InsertRobotEntity> robotAccounts, bool isRescan);

        void InsertPALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName);

        void RemovePALogics(IEnumerable<CustRemovePAEntity> customerInfo, string serviceName);

        void InsertRobotLogics(IEnumerable<CustInfoEntity> customerInfo);

        bool CheckPAToUnflagBetlimit(IEnumerable<long> ctsCustIds, string serviceName);
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

        public IEnumerable<long> InsertProblemAccountPackage(IEnumerable<InsertProblemAccountEntity> problemAccounts, FunctionId funcId, InsertPA_ByType insertByType, bool isToQueue, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var paLosing = GetProblemAccountLosingClassification(problemAccounts.Select(c => c.CustID).Distinct(), funcId);

                var paClassification = MergePALosingClassification(problemAccounts, paLosing);

                var custInfo = InsertProblemAccount(paClassification, insertByType, isToQueue);

                if (custInfo != null && custInfo.Any())
                {
                    InsertPALogics(custInfo, serviceName);

                    return custInfo.Select(c => c.CustID).Distinct();
                }
            }

            return Enumerable.Empty<long>();
        }

        public RemovePAResultEntity RemoveProblemAccountPackage(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName)
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

        public void RemovePALogics(IEnumerable<CustRemovePAEntity> customerInfo, string serviceName)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custIds = customerInfo.Where(c => c.IsCleanPA).Select(x => x.CustID).Distinct();

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

        public IEnumerable<CustInfoEntity> InsertRobotAccountPackage(IEnumerable<InsertRobotEntity> robotAccounts, bool isRescan)
        {
            if (robotAccounts != null && robotAccounts.Any())
            {
                var paLosing = GetProblemAccountLosingClassification(robotAccounts.Select(c => c.CustID).Distinct(), FunctionId.RobotUserJob);

                var robotClassification = MergeRobotLosingClassification(robotAccounts, paLosing);

                var customerInfo = InsertRobotAccount(robotClassification, isRescan);

                InsertRobotLogics(customerInfo);

                return customerInfo;
            }

            return Enumerable.Empty<CustInfoEntity>();
        }

        public void InsertRobotLogics(IEnumerable<CustInfoEntity> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                CheckNewPAAssociationCustomerForRollback(customerInfo);
            }
        }

        public bool CheckPAToUnflagBetlimit(IEnumerable<long> ctsCustIds, string serviceName)
        {
            if (ctsCustIds != null && ctsCustIds.Any())
            {
                var custCleanPA = CheckCleanCustFlaggedEvidence(ctsCustIds);

                if (custCleanPA != null && custCleanPA.Any())
                {
                    UpdateBetLimitOfUnflaggedCustomer(custCleanPA?.Select(c => c.CustID)?.Distinct(), serviceName);
                }
            }

            return true;
        }

        #region Private Methods

        private IEnumerable<CustInfoEntity> InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> problemAccounts, InsertPA_ByType insertByType, bool isToQueue)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccountDataAccess.InsertProblemAccount(JsonConvert.SerializeObject(problemAccounts), Enum.GetName(typeof(InsertPA_ByType), insertByType), isToQueue);
            }

            return Enumerable.Empty<CustInfoEntity>();
        }

        private RemovePAResultEntity RemoveProblemAccount(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = problemAccountDataAccess.RemoveProblemAccount(problemAccounts, isFromQueue);

                // hard code for test
                Utilities.LogSentryInfo(serviceName, "RemoveProblemAccount", result.CustomerInfo, true, sentryClient);
                // hard code for test end

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "RemoveProblemAccount", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
                }

                return result;
            }

            return new RemovePAResultEntity();
        }

        private IEnumerable<CustInfoEntity> InsertRobotAccount(IEnumerable<InsertRobotEntity> robotAccounts, bool isRescan)
        {
            if (robotAccounts != null && robotAccounts.Any())
            {               
                return problemAccountDataAccess.InsertRobotAccount(JsonConvert.SerializeObject(robotAccounts), isRescan);
            }

            return Enumerable.Empty<CustInfoEntity>();
        }

        private void CheckNewPAAssociationCustomerForRollback(IEnumerable<CustInfoEntity> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);
            }
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

        private IEnumerable<CustInfoEntity> CheckCleanCustFlaggedEvidence(IEnumerable<long> ctsCustIds)
        {
            if (ctsCustIds != null && ctsCustIds.Any())
            {
                var ctsCustIdsToString = string.Join(",", ctsCustIds.Distinct());

                return problemAccountDataAccess.CheckCleanPAEvidence(ctsCustIdsToString);
            }

            return Enumerable.Empty<CustInfoEntity>();
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
                                                            CreatedBy = group.pa.CreatedBy,
                                                            LastModifiedDate = group.pa.LastModifiedDate,
                                                            Remark = group.pa.Remark,
                                                            MatchID = group.pa.MatchID,
                                                           
                                                            IsDirectPA = group.pa.IsDirectPA,
                                                            IsFromCTS = group.pa.IsFromCTS,
                                                            IsFromTVS = group.pa.IsFromTVS,
                                                            TVSRequestID = group.pa.TVSRequestID,
                                                            IsFromTW = group.pa.IsFromTW,
                                                            IsDataChanged = group.pa.IsDataChanged,
                                                            ReasonID = group.pa.ReasonID,
                                                            SportType = group.pa.SportType,
                                                            IsParlay = group.pa.IsParlay,
                                                            IssueTypeID = group.pa.IssueTypeID,

                                                            BetCount = temp?.BetCount,
                                                            ActiveDays = temp?.ActiveDays,
                                                            TurnoverRM = temp?.TurnoverRM,
                                                            WinlossRM = temp?.WinlossRM,
                                                            WinlossStatus = temp?.WinlossStatus                                                            
                                                        });
            }

            return Enumerable.Empty<InsertProblemAccountEntity>();
        }

        private IEnumerable<InsertRobotEntity> MergeRobotLosingClassification(IEnumerable<InsertRobotEntity> robotAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> paLosing)
        {
            if (robotAccounts != null && robotAccounts.Any())
            {
                return robotAccounts.GroupJoin(paLosing ?? Enumerable.Empty<ProblemAccountLosingClassificationEntity>(),
                                                 pa => pa.CustID,
                                                 pl => pl.CustId,
                                                (pa, pl) => new { pa, pl })
                                                .SelectMany(
                                                        group => group.pl.DefaultIfEmpty(),
                                                        (group, temp) => new InsertRobotEntity
                                                        {
                                                            CustID = group.pa.CustID,
                                                            CategoryID = group.pa.CategoryID,
                                                            RobotCounter = group.pa.RobotCounter,
                                                            Remark = group.pa.IsFromImperva ? "Imperva" : group.pa.Remark,
                                                            CreatedBy = group.pa.CreatedBy,

                                                            IsFromCTS = group.pa.IsFromCTS,
                                                            IsFromAI = group.pa.IsFromAI,
                                                            IsFromImperva = group.pa.IsFromImperva,
                                                            IsFromTVS = group.pa.IsFromTVS,
                                                            IsFromTW = group.pa.IsFromTW,
                                                            TVSRequestID = group.pa.TVSRequestID,
                                                            IsDirect = group.pa.IsDirect,
                                                            SportType = group.pa.SportType,
                                                            IsParlay = group.pa.IsParlay,
                                                            IssueTypeID = group.pa.IssueTypeID,

                                                            BetCount = temp?.BetCount,
                                                            ActiveDays = temp?.ActiveDays,
                                                            TurnoverRM = temp?.TurnoverRM,
                                                            WinlossRM = temp?.WinlossRM,
                                                            WinlossStatus = temp?.WinlossStatus,
                                                        });
            }

            return Enumerable.Empty<InsertRobotEntity>();
        }

        #endregion Private Methods
    }
}