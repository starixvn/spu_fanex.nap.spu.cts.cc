namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAffectedDownlineCustomerClassificationService
    {
        IAffectedDownlineCustomerClassificationService Start();

        IAffectedDownlineCustomerClassificationService GetCustomerClassificationFromQueue();

        IAffectedDownlineCustomerClassificationService CustomerClassification(RescanParametersEntity parameters);

        IEnumerable<LatestClassificationEntity> InsertVVIPCategory(VVIPCustomerInsertEntity custInfo);

        IEnumerable<VVIPCustomerRemoveResultEntity> RemoveVVIPCategory(VVIPCustomerInsertEntity model);

        bool CompleteCustomerClassificationFromQueue();
    }

    public class AffectedDownlineCustomerClassificationService : IAffectedDownlineCustomerClassificationService
    {
        private readonly IAffectedDownlineCustomerClassificationDataAccess affectedCustClassificationDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IDailyProblemClassificationServices dailyProblemClassificationServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private CustClassificationFromQueueResultEntity custClassificationFromQueue;
        private bool IsSuccess;

        public AffectedDownlineCustomerClassificationService(
            IAffectedDownlineCustomerClassificationDataAccess affectedCustClassificationDataAccess,
            IProblemAccountServices problemAccountServices,
            IDailyProblemClassificationServices dailyProblemClassificationServices,
            ICustClassToMainDBService custClassToMainDBService,
            IAntiFraudCenterServices antiFraudCenterServices,
            IRescanCustClassificationService rescanCustClassificationService)
        {
            this.affectedCustClassificationDataAccess = affectedCustClassificationDataAccess;
            this.problemAccountServices = problemAccountServices;
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.rescanCustClassificationService = rescanCustClassificationService;
        }

        public IAffectedDownlineCustomerClassificationService Start()
        {
            custClassificationFromQueue = new CustClassificationFromQueueResultEntity();
            IsSuccess = true;

            return this;
        }

        public IAffectedDownlineCustomerClassificationService GetCustomerClassificationFromQueue()
        {
            custClassificationFromQueue = affectedCustClassificationDataAccess.GetCustomerCategoryFromQueue();

            return this;
        }


        public IAffectedDownlineCustomerClassificationService CustomerClassification(RescanParametersEntity parameters)
        {
            if (custClassificationFromQueue != null
                 && custClassificationFromQueue.CustomerClassificationQueue != null
                 && custClassificationFromQueue.CustomerClassificationQueue.Any())
            {
                var actionType = custClassificationFromQueue.ActionType;

                switch (actionType)
                {
                    case (int)DownlineClassification_ActionType.InsertVVIP:
                        {
                            var custInfo = new VVIPCustomerInsertEntity
                            {
                                FromAction = VVIP_FromAction.Service,
                                IsAffectDownline = false,
                                QueueID = Convert.ToInt64(custClassificationFromQueue.QueueID),
                                UplineRoleID = custClassificationFromQueue.UplineRoleID,
                                CustInfo = custClassificationFromQueue.CustomerClassificationQueue.Select(c => new VVIPCategoryEntity
                                {
                                    CustID = c.CustID,
                                    CTSCustID = c.CTSCustID,
                                    SubscriberID = c.SubscriberID,
                                    RoleID = c.RoleID,
                                    Remark = c.Remark,
                                    CreatedBy = c.CreatedBy
                                })
                            };

                            InsertVVIPForDownline(parameters, custInfo);

                            break;
                        }
                    case (int)DownlineClassification_ActionType.RemoveVVIP:
                        {
                            var custInfo = new VVIPCustomerInsertEntity
                            {
                                FromAction = VVIP_FromAction.Service,
                                IsAffectDownline = false,
                                QueueID = Convert.ToInt64(custClassificationFromQueue.QueueID),
                                UplineRoleID = custClassificationFromQueue.UplineRoleID,
                                CustInfo = custClassificationFromQueue.CustomerClassificationQueue.Select(c => new VVIPCategoryEntity
                                {
                                    CustID = c.CustID,
                                    CTSCustID = c.CTSCustID,
                                    SubscriberID = c.SubscriberID,
                                    RoleID = c.RoleID,
                                    Remark = c.Remark,
                                    CreatedBy = c.CreatedBy
                                })
                            };

                            RemoveVVIPForDownline(parameters, custInfo);
                            break;
                        }
                    case (int)DownlineClassification_ActionType.InsertPA:
                        {
                            InsertDownlineProblemAccount(parameters);
                            break;
                        }
                    case (int)DownlineClassification_ActionType.RemovePA:
                        {
                            RemoveDownlineProblemAccount(parameters);
                            break;
                        } 
                    default:
                        break;
                }    
            }

            return this;
        }

        public IAffectedDownlineCustomerClassificationService InsertDownlineProblemAccount(RescanParametersEntity parameters)
        {
            if (custClassificationFromQueue.CustomerClassificationQueue != null && custClassificationFromQueue.CustomerClassificationQueue.Any())
            {
                var custIds = custClassificationFromQueue.CustomerClassificationQueue.Select(c => c.CustID).Distinct();

                var probAccounts = custClassificationFromQueue.CustomerClassificationQueue.Select(c => new InsertProblemAccountEntity
                {
                    CustID = c.CustID,
                    CTSCustID = c.CTSCustID,
                    RoleID = c.RoleID,
                    SubscriberID = c.SubscriberID,
                    CategoryID = c.CategoryID,
                    CategoryGroup = c.CategoryGroup,
                    IsLicensee = c.IsLicensee,
                    Remark = c.Remark,
                    IsDirectPA = false,
                    IsFromCTS = c.IsFromCTS,
                    IsFromTVS = c.IsFromTVS,
                    IsFromTW = c.IsFromTW,
                    IsDataChanged = false,
                    CreatedBy = c.CreatedBy

                });

                var paLosing = dailyProblemClassificationServices.GetProblemAccountLosingClassification(custIds, FunctionId.DownlineCustomerClassification);

                var insertPAResult = problemAccountServices.InsertProblemAccountLogics(probAccounts, paLosing, InsertPA_ByType.Category, true, parameters.ServiceName);

                IsSuccess = insertPAResult.ErrorCode == 0;

                if (IsSuccess && insertPAResult.CustomerInfo.Any())
                {
                    var custToPushExternal = insertPAResult.CustomerInfo.Select(c => c.CustID).Distinct();

                    Parallel.Invoke(() =>
                    {
                        IsSuccess = custClassToMainDBService.PushDataToExternal(custToPushExternal, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = parameters.ExternalBatchSize,
                            FeedSoon88API = parameters.FeedSoon88API,
                            IsPushCCToSoon88 = parameters.IsPushCCToSoon88,
                            IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB,
                            IsWriteLog = parameters.IsWriteLog,
                            ServiceName = parameters.ServiceName
                        });
                    },
                    () =>
                    {
                        antiFraudCenterServices.InsertAFCAccounts(custToPushExternal, FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCInsertAccountURL);
                    });
                }
            }           

            return this;
        }

        public IAffectedDownlineCustomerClassificationService RemoveDownlineProblemAccount(RescanParametersEntity parameters)
        {
            if (custClassificationFromQueue.CustomerClassificationQueue != null && custClassificationFromQueue.CustomerClassificationQueue.Any())
            {
                var probAccounts = custClassificationFromQueue.CustomerClassificationQueue.Select(c => new RemoveProblemAccountEntity
                {
                    CustID = c.CustID,
                    CTSCustID = c.CTSCustID,
                    RoleID = c.RoleID,
                    SubscriberID = c.SubscriberID,
                    Remark = c.Remark,
                    IsDirectPA = false,
                    IsExceptDirectDownline = c.IsExceptDirectDownline,
                    CreatedBy = c.CreatedBy
                });

                var result = problemAccountServices.RemoveProblemAccountLogics(probAccounts, true, parameters.ServiceName);

                IsSuccess = result.ErrorCode == 0;

                if (IsSuccess && result.CustomerInfo != null && result.CustomerInfo.Any())
                {
                    IsSuccess = rescanCustClassificationService.RescanCustomerClassification(parameters);
                }
            }

            return this;
        }

        public bool CompleteCustomerClassificationFromQueue()
        {
            if (IsSuccess  
                 && custClassificationFromQueue != null
                 && custClassificationFromQueue.CustomerClassificationQueue != null
                 && custClassificationFromQueue.CustomerClassificationQueue.Any())
            {
                affectedCustClassificationDataAccess.CustomerClassificationFromQueueComplete(custClassificationFromQueue.QueueID, custClassificationFromQueue.LastDownlineCTSCustID);
            }

            return IsSuccess;
        }

        public bool InsertVVIPForDownline(RescanParametersEntity parameters, VVIPCustomerInsertEntity custInfo)
        {
            var isSuccess = true;

            if (custInfo != null && custInfo.CustInfo.Any())
            {
                var result = InsertVVIPCategory(custInfo);

                isSuccess = custClassToMainDBService.RetryPushCCToMainDB(result, Constants.RetryPushCCToExternal, parameters.ServiceName);

                var custIds = result.Select(c => c.CustId).Distinct();
                antiFraudCenterServices.DeleteAFCAccounts(custIds, FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCDeleteAccountURL);
            }

            return isSuccess;
        }

        public bool RemoveVVIPForDownline(RescanParametersEntity parameters, VVIPCustomerInsertEntity custInfo)
        {
            var isSuccess = true;

            if (custInfo != null && custInfo.CustInfo.Any())
            {
                var result = RemoveVVIPCategory(custInfo);

                parameters.CustIds = result.Select(c => c.CustID).Distinct();
                isSuccess = rescanCustClassificationService.RescanCustomerClassification(parameters);
            }

            return isSuccess;
        }

        public IEnumerable<LatestClassificationEntity> InsertVVIPCategory(VVIPCustomerInsertEntity custInfo)
        {
            if (custInfo != null && custInfo.CustInfo != null && custInfo.CustInfo.Any())
            {
                return affectedCustClassificationDataAccess.InsertVVIPCategory(custInfo);
            }

            return Enumerable.Empty<LatestClassificationEntity>();
        }

        public IEnumerable<VVIPCustomerRemoveResultEntity> RemoveVVIPCategory(VVIPCustomerInsertEntity custInfo)
        {
            if (custInfo != null && custInfo.CustInfo != null && custInfo.CustInfo.Any())
            {
                return affectedCustClassificationDataAccess.RemoveVVIPCategory(custInfo);
            }

            return Enumerable.Empty<VVIPCustomerRemoveResultEntity>();
        }
    }
}
