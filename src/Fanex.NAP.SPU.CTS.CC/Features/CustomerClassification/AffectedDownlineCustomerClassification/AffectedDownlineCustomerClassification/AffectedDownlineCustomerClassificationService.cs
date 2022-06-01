namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAffectedDownlineCustomerClassificationService
    {
        IAffectedDownlineCustomerClassificationService Start();

        IAffectedDownlineCustomerClassificationService GetCustomerClassificationFromQueue();

        IAffectedDownlineCustomerClassificationService CustomerClassification(RescanParametersEntity parameters);

        bool InsertVVIPForDownline(RescanParametersEntity parameters, VVIPCustomerInsertEntity custInfo);

        bool CompleteCustomerClassificationFromQueue();

        IEnumerable<LatestClassificationEntity> InsertVVIPCategory(VVIPCustomerInsertEntity custInfo);

        IEnumerable<VVIPCustomerRemoveResultEntity> RemoveVVIPCategory(VVIPCustomerInsertEntity model);


    }

    public class AffectedDownlineCustomerClassificationService : IAffectedDownlineCustomerClassificationService
    {
        private readonly IAffectedDownlineCustomerClassificationDataAccess affectedCustClassificationDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private CustClassificationFromQueueResultEntity custClassificationFromQueue;
        private bool IsSuccess;

        public AffectedDownlineCustomerClassificationService(
            IAffectedDownlineCustomerClassificationDataAccess affectedCustClassificationDataAccess,
            IProblemAccountServices problemAccountServices,
            ICustClassToMainDBService custClassToMainDBService,
            IAntiFraudCenterServices antiFraudCenterServices,
            IRescanCustClassificationService rescanCustClassificationService)
        {
            this.affectedCustClassificationDataAccess = affectedCustClassificationDataAccess;
            this.problemAccountServices = problemAccountServices;
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
                            IsSuccess = InsertVVIPForDownline(parameters, MappingVVIPModel(custClassificationFromQueue));
                            break;
                        }
                    case (int)DownlineClassification_ActionType.RemoveVVIP:
                        {
                            IsSuccess = RemoveVVIPForDownline(parameters, MappingVVIPModel(custClassificationFromQueue));
                            break;
                        }
                    case (int)DownlineClassification_ActionType.InsertPA:
                        {
                            IsSuccess = InsertDownlineProblemAccount(parameters, custClassificationFromQueue);
                            break;
                        }
                    case (int)DownlineClassification_ActionType.RemovePA:
                        {
                            IsSuccess = RemoveDownlineProblemAccount(parameters, custClassificationFromQueue);
                            break;
                        } 
                }    
            }

            return this;
        }

        public bool InsertDownlineProblemAccount(RescanParametersEntity parameters, CustClassificationFromQueueResultEntity custInfo)
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

                var insertPAResult = problemAccountServices.InsertProblemAccountLogics(probAccounts, FunctionId.DownlineCustomerClassification, InsertPA_ByType.Category, true, parameters.ServiceName);
                
                return custClassToMainDBService.PushExternalProblemAcountPackage(insertPAResult, parameters);
            }           

            return true;
        }

        public bool RemoveDownlineProblemAccount(RescanParametersEntity parameters, CustClassificationFromQueueResultEntity custInfo)
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
                    IsDirectPA = false,
                    IsExceptDirectDownline = c.IsExceptDirectDownline,
                    CreatedBy = c.CreatedBy
                });

                var result = problemAccountServices.RemoveProblemAccountLogics(probAccounts, true, parameters.ServiceName);

                isSuccess = result.ErrorCode == 0;

                if (isSuccess && result.CustomerInfo != null && result.CustomerInfo.Any())
                {
                    parameters.IsRescanPA = false;
                    isSuccess = rescanCustClassificationService.RescanCustomerClassification(parameters);
                }
            }

            return isSuccess;
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
                parameters.IsRescanPA = true;
                isSuccess = rescanCustClassificationService.RescanCustomerClassification(parameters);
            }

            return isSuccess;
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

        public static VVIPCustomerInsertEntity MappingVVIPModel (CustClassificationFromQueueResultEntity custInfo)
        {
            return new VVIPCustomerInsertEntity
            {
                FromAction = VVIP_FromAction.Service,
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
