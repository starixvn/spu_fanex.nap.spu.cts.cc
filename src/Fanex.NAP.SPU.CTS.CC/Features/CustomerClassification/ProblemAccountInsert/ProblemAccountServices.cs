namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
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
        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId);

        IEnumerable<long> RescanCustomerClassification(IEnumerable<GetRescanCustomerClassificationEntity> custRescans, FunctionId funcId, CCInputFlow ccInputFlow, bool isToQueue, string serviceName);

        IEnumerable<long> InsertProblemAccountPackage(IEnumerable<InsertProblemAccountEntity> problemAccounts, FunctionId funcId, CCInputFlow ccInputFlow, bool isToQueue, string serviceName);
     
        ResultRemoveCustomerClassificationModel RemoveProblemAccountPackage(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName);

        void InsertPALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName);
       
        void InsertRobotLogics(IEnumerable<CustInfoEntity> customerInfo);        
    }

    public class ProblemAccountServices : BaseCustomerClassificationService, IProblemAccountServices
    {
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly ISentryClient sentryClient;

        public ProblemAccountServices(IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ISentryClient sentryClient) : base(baseDataAccess, sentryClient)
        {
            this.problemAccountDataAccess = problemAccountDataAccess;
            this.sentryClient = sentryClient;
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

        public IEnumerable<long> RescanCustomerClassification(IEnumerable<GetRescanCustomerClassificationEntity> custRescans, FunctionId funcId, CCInputFlow ccInputFlow, bool isToQueue, string serviceName)
        {
            var pas = custRescans.Select(x => new InsertProblemAccountEntity(x));

            return InsertProblemAccountPackage(pas, funcId, ccInputFlow, isToQueue, serviceName);
        }

        public IEnumerable<long> InsertProblemAccountPackage(IEnumerable<InsertProblemAccountEntity> problemAccounts, FunctionId funcId, CCInputFlow ccInputFlow, bool isToQueue, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var paClassification = CombinePAWithLosingPerformace(problemAccounts, funcId);

                var modelInsert = new InsertCustomerClassificationModel()
                {
                    CustInfoJson = JsonConvert.SerializeObject(paClassification),
                    IsToQueue = isToQueue,
                    FromAction = (int)FromFlow.CTSServices
                };

                var insertPaResult = base.InsertCustomerClassification(ccInputFlow, modelInsert, serviceName);

                if (insertPaResult.CustInfos != null && insertPaResult.CustInfos.Any())
                {
                    var custUpdatePALogics = insertPaResult.CustInfos.Select(x => new CustInfoEntity { CustID = x.CustID, CTSCustID = x.CTSCustID, IsPA = x.IsPA });

                    InsertPALogics(custUpdatePALogics, serviceName);                    

                    return insertPaResult.CustInfos.Select(x => x.CustID).Distinct();
                }
            }

            return Enumerable.Empty<long>();
        }       

        public ResultRemoveCustomerClassificationModel RemoveProblemAccountPackage(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName)
        {
            var result = new ResultRemoveCustomerClassificationModel();

            if (problemAccounts != null && problemAccounts.Any())
            {
                result = RemoveProblemAccount(problemAccounts, isFromQueue, serviceName);

                if (string.IsNullOrEmpty(result.ErrorMessage))
                {
                    RemovePALogics(result.CustInfos, serviceName);
                }
            }

            return result;
        }

        public void InsertPALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName)
        {            
            if (customerInfo != null && customerInfo.Any())
            {
                var custPas = customerInfo.Where(x => x.IsPA).Distinct();
               
                CheckNewPAAssociationCustomerForRollback(custPas);

                UpdateBetLimitOfFlaggedCustomer(custPas.Select(x => x.CustID), serviceName);
            }
        }
        
        public void InsertRobotLogics(IEnumerable<CustInfoEntity> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                CheckNewPAAssociationCustomerForRollback(customerInfo);
            }
        }

        #region Private Methods
        private void RemovePALogics(IEnumerable<ResultRemoveCustomerClassificationEntity> customerInfo, string serviceName)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custIds = customerInfo.Where(c => c.IsCleanPA).Select(x => x.CustID).Distinct();

                UpdateBetLimitOfUnflaggedCustomer(custIds, serviceName);
            }
        }

        private IEnumerable<InsertProblemAccountEntity> CombinePAWithLosingPerformace(IEnumerable<InsertProblemAccountEntity> problemAccounts, FunctionId funcId)
        {
            var custIds = problemAccounts.Select(c => c.CustID)?.Distinct();

            var paLosing = GetProblemAccountLosingClassification(custIds, funcId);

            return MergePALosingClassification(problemAccounts, paLosing);
        }

        private ResultRemoveCustomerClassificationModel RemoveProblemAccount(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue, string serviceName)
        {
            var removedModel = new RemoveCustomerClassificationModel()
            {
                CustInfoJson = JsonConvert.SerializeObject(problemAccounts),
                IsFromQueue = isFromQueue,
                FromAction = (int)FromFlow.CTSServices
            };

            var resultRemove = base.RemoveCustomerClassification(CCInputFlow.RemovePACategory, removedModel, serviceName);

            return resultRemove;
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
                                                            IsLicensee = group.pa.IsLicensee,
                                                            CreatedBy = group.pa.CreatedBy,
                                                            LastModifiedDate = group.pa.LastModifiedDate,
                                                            CreatedDate = group.pa.CreatedDate,
                                                            Remark = group.pa.Remark,
                                                            MatchID = group.pa.MatchID,

                                                            IsMarkedDirectly = group.pa.IsMarkedDirectly,
                                                            IsFromAI = group.pa.IsFromAI,
                                                            IsFromCTS = group.pa.IsFromCTS,
                                                            IsFromTVS = group.pa.IsFromTVS,
                                                            TVSRequestID = group.pa.TVSRequestID,
                                                            IsFromTW = group.pa.IsFromTW,
                                                            TVSReasonID = group.pa.TVSReasonID,
                                                            SportType = group.pa.SportType,
                                                            SportName = group.pa.SportName,
                                                            GBTicketCount = group.pa.GBTicketCount,
                                                            DetectedDate = group.pa.DetectedDate,
                                                            IsParlay = group.pa.IsParlay,
                                                            IssueTypeID = group.pa.IssueTypeID,
                                                            RobotCounter = group.pa.RobotCounter,
                                                            GroupName = group.pa.GroupName,

                                                            BetCount = temp?.BetCount,
                                                            ActiveDays = temp?.ActiveDays,
                                                            TurnoverRM = temp?.TurnoverRM,
                                                            WinlossRM = temp?.WinlossRM,
                                                            WinlossStatus = temp?.WinlossStatus,
                                                            PerformanceTime = temp?.PerformanceTime
                                                        });
            }

            return Enumerable.Empty<InsertProblemAccountEntity>();
        }
        
        #endregion Private Methods
    }
}