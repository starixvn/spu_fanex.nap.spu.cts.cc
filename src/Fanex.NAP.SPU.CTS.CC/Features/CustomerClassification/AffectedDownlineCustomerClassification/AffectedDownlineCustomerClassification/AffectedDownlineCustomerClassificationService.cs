namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAffectedDownlineCustomerClassificationService
    {
        IAffectedDownlineCustomerClassificationService Start();

        IAffectedDownlineCustomerClassificationService GetCustomerClassificationFromQueue();

        IAffectedDownlineCustomerClassificationService CustomerClassification(RescanParametersEntity parameters);

        bool CompleteCustomerClassificationFromQueue();

        bool InsertVVIP(RescanParametersEntity parameters, VVIPCustomerInsertEntity custInfo);
    }

    public class AffectedDownlineCustomerClassificationService : ProblemAccountServices, IAffectedDownlineCustomerClassificationService
    {
        private readonly IAffectedDownlineCustomerClassificationDataAccess affectedCustClassificationDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private CustClassificationFromQueueResultEntity dataFromQueue;
        private bool IsSuccess;

        public AffectedDownlineCustomerClassificationService(
            IAffectedDownlineCustomerClassificationDataAccess affectedCustClassificationDataAccess,
            IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ICustClassToMainDBService custClassToMainDBService,
            IAntiFraudCenterServices antiFraudCenterServices,
            IRescanCustClassificationService rescanCustClassificationService,
            ISentryClient sentryClient) : base(baseDataAccess, problemAccountDataAccess, sentryClient)
        {
            this.affectedCustClassificationDataAccess = affectedCustClassificationDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.rescanCustClassificationService = rescanCustClassificationService;
        }

        public IAffectedDownlineCustomerClassificationService Start()
        {
            dataFromQueue = new CustClassificationFromQueueResultEntity();
            IsSuccess = true;
            return this;
        }

        public IAffectedDownlineCustomerClassificationService GetCustomerClassificationFromQueue()
        {
            dataFromQueue = affectedCustClassificationDataAccess.GetCustomerCategoryFromQueue();

            return this;
        }

        public IAffectedDownlineCustomerClassificationService CustomerClassification(RescanParametersEntity parameters)
        {
            if (dataFromQueue != null
                 && dataFromQueue.CustomerClassificationQueue != null
                 && dataFromQueue.CustomerClassificationQueue.Any())
            {
                var actionType = dataFromQueue.ActionType;

                switch (actionType)
                {
                    case (int)DownlineClassification_ActionType.InsertVVIP:
                        {
                            IsSuccess = InsertVVIP(parameters, MappingVVIPModel(dataFromQueue));
                            break;
                        }
                    case (int)DownlineClassification_ActionType.RemoveVVIP:
                        {
                            IsSuccess = RemoveVVIP(parameters, MappingVVIPModel(dataFromQueue));
                            break;
                        }
                    case (int)DownlineClassification_ActionType.InsertPA:
                        {
                            IsSuccess = InsertPA(parameters, dataFromQueue);
                            break;
                        }
                    case (int)DownlineClassification_ActionType.RemovePA:
                        {
                            IsSuccess = RemovePA(parameters, dataFromQueue);
                            break;
                        }
                }
            }

            return this;
        }

        public bool CompleteCustomerClassificationFromQueue()
        {
            if (IsSuccess && dataFromQueue != null)
            {
                affectedCustClassificationDataAccess.CustomerClassificationFromQueueComplete(dataFromQueue.QueueID, dataFromQueue.LastDownlineCTSCustID);
            }

            return IsSuccess;
        }

        private bool InsertPA(RescanParametersEntity parameters, CustClassificationFromQueueResultEntity custInfo)
        {
            if (custInfo != null && custInfo.CustomerClassificationQueue.Any())
            {
                var custIds = custInfo.CustomerClassificationQueue.Select(c => c.CustID).Distinct();

                var probAccounts = custInfo.CustomerClassificationQueue.Select(c => new InsertProblemAccountEntity
                {
                    CustID = c.CustID,
                    CTSCustID = c.CTSCustID,
                    RoleID = c.RoleID,
                    SubscriberID = c.SubscriberID,
                    CategoryID = c.CategoryID,
                    CategoryGroupID = c.CategoryGroup,
                    IsLicensee = c.IsLicensee,
                    Remark = c.Remark,
                    IsMarkedDirectly = false,
                    IsFromCTS = c.IsFromCTS,
                    IsFromTVS = c.IsFromTVS,
                    IsFromTW = c.IsFromTW,
                    TVSRequestID = c.TVSRequestID,
                    CreatedBy = c.CreatedBy
                });

                var result = InsertProblemAccountPackage(probAccounts, FunctionId.DownlineCustomerClassification, CCInputFlow.InsertPACategory, parameters.IsToQueue, parameters.ServiceName);

                return custClassToMainDBService.PushExternalProblemAcountPackage(result, parameters.PushDataExternal);
            }

            return true;
        }

        private bool RemovePA(RescanParametersEntity rescanConfig, CustClassificationFromQueueResultEntity custInfo)
        {
            var isSuccess = true;

            if (custInfo.CustomerClassificationQueue != null && custInfo.CustomerClassificationQueue.Any())
            {
                var probAccounts = custInfo.CustomerClassificationQueue.Select(c => new RemoveProblemAccountEntity
                {
                    CustID = c.CustID,
                    CTSCustID = c.CTSCustID,
                    RoleID = c.RoleID,
                    SubscriberID = c.SubscriberID,
                    Remark = c.Remark,
                    IsMarkedDirectly = true,
                    IsExceptDirectDownline = c.IsExceptDirectDownline,
                    CreatedBy = c.CreatedBy
                });

                var result = RemoveProblemAccountPackage(probAccounts, true, rescanConfig.ServiceName);

                isSuccess = string.IsNullOrEmpty(result.ErrorMessage);

                if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustInfos != null && result.CustInfos.Any())
                {
                    isSuccess = rescanCustClassificationService.RescanCustomerClassification(result.CustInfos.Select(c => c.CustID).Distinct(), rescanConfig);
                }
            }

            return isSuccess;
        }

        public bool InsertVVIP(RescanParametersEntity parameters, VVIPCustomerInsertEntity custInfo)
        {
            var isSuccess = true;

            if (custInfo != null && custInfo.CustInfo.Any())
            {
                var modelInsert = new InsertCustomerClassificationModel()
                {
                    CustInfoJson = JsonConvert.SerializeObject(custInfo.CustInfo),
                    FromAction = (int)custInfo.FromFlow,
                    UplineRoleID = custInfo.UplineRoleID,
                    QueueID = custInfo.QueueID,
                    IsAffectDownline = custInfo.IsAffectDownline,
                    IsToQueue = parameters.IsToQueue
                };

                var result = base.InsertCustomerClassification(CCInputFlow.VVIP, modelInsert, parameters.ServiceName);

                isSuccess = string.IsNullOrEmpty(result.ErrorMessage);

                if (result.CustInfos != null && result.CustInfos.Any())
                {
                    var custIDs = result.CustInfos.Select(c => c.CustID).Distinct();

                    isSuccess = custClassToMainDBService.PushExternalPackageByCustId(custIDs, new PushDataToExternalEntity
                    {
                        ExternalBatchSize = parameters.PushDataExternal.ExternalBatchSize,
                        IsUpdateDangerToMainDB = false,
                        IsSyncAFCData = false,
                        ServiceName = parameters.ServiceName
                    });

                    antiFraudCenterServices.DeleteAFCAccounts(custIDs, FromFlow.CTSServices, parameters.PushDataExternal.IsSyncAFCData, parameters.PushDataExternal.AFCDeleteAccountURL);
                }
            }

            return isSuccess;
        }

        private bool RemoveVVIP(RescanParametersEntity rescanConfig, VVIPCustomerInsertEntity custInfo)
        {
            var isSuccess = true;

            if (custInfo != null && custInfo.CustInfo.Any())
            {
                var modelInsert = new RemoveCustomerClassificationModel()
                {
                    CustInfoJson = JsonConvert.SerializeObject(custInfo.CustInfo.Select(c => new RemoveCustomerClassificationEntity
                    {
                        CustID = c.CustID,
                        CTSCustID = c.CTSCustID,
                        CreatedBy = c.CreatedBy
                    })),
                    FromAction = (int)custInfo.FromFlow,
                    UplineRoleID = custInfo.UplineRoleID,
                    QueueID = custInfo.QueueID,
                    IsAffectDownline = custInfo.IsAffectDownline,
                    IsFromQueue = false
                };

                var result = base.RemoveCustomerClassification(CCInputFlow.RemoveVVIP, modelInsert, rescanConfig.ServiceName);

                isSuccess = string.IsNullOrEmpty(result.ErrorMessage);

                if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustInfos != null && result.CustInfos.Any())
                {
                    isSuccess = rescanCustClassificationService.RescanCustomerClassification(result.CustInfos.Select(c => c.CustID).Distinct(), rescanConfig);
                }
            }

            return isSuccess;
        }

        private VVIPCustomerInsertEntity MappingVVIPModel(CustClassificationFromQueueResultEntity custInfo)
        {
            return new VVIPCustomerInsertEntity
            {
                FromFlow = FromFlow.CTSServices,
                IsAffectDownline = false,
                QueueID = Convert.ToInt64(custInfo.QueueID),
                UplineRoleID = custInfo.UplineRoleID,
                CustInfo = custInfo.CustomerClassificationQueue.Select(c => new VVIPCategoryEntity
                {
                    CustID = c.CustID,
                    CTSCustID = c.CTSCustID,
                    SubscriberID = c.SubscriberID,
                    RoleID = c.RoleID,
                    Remark = c.Remark,
                    CreatedBy = c.CreatedBy
                })
            };
        }
    }
}