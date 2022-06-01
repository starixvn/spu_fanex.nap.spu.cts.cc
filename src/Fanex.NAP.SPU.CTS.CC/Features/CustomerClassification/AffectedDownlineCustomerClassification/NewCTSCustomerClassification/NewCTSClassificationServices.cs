namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INewCTSClassificationServices
    {
        INewCTSClassificationServices Start();

        INewCTSClassificationServices GetCategoryForNewCTSCustomer();

        INewCTSClassificationServices InsertPAForNewCustomers(RescanParametersEntity parameters);

        INewCTSClassificationServices InsertVVIPForNewCustomers(RescanParametersEntity parameters);

        INewCTSClassificationServices ResanNormalForNewCustomers(RescanParametersEntity parameters);

        bool UpdateComplete();

    }

    public class NewCTSClassificationServices : INewCTSClassificationServices
    {
        private readonly INewCTSClassificationDataAccess newCTSClassificationDataAccess;
        private readonly IAffectedDownlineCustomerClassificationService affectedCustomerClassificationService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISystemParameterServices systemParameterServices;
        private NewCTSCustomerClassificationEntity newCustomerClassification;
        private bool IsSuccess;

        public NewCTSClassificationServices(
                INewCTSClassificationDataAccess newCTSClassificationDataAccess,
                IAffectedDownlineCustomerClassificationService affectedCustomerClassificationService,
                IDailyProblemClassificationServices dailyProblemClassificationServices,
                IProblemAccountServices problemAccountServices,
                IRescanCustClassificationService rescanCustClassificationService,
                ICustClassToMainDBService custClassToMainDBService,
                IAntiFraudCenterServices antiFraudCenterServices,
                ISystemParameterServices systemParameterServices)
        {
            this.newCTSClassificationDataAccess = newCTSClassificationDataAccess;
            this.affectedCustomerClassificationService = affectedCustomerClassificationService;
            this.problemAccountServices = problemAccountServices;
            this.rescanCustClassificationService = rescanCustClassificationService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.systemParameterServices = systemParameterServices;
        }

        public INewCTSClassificationServices Start()
        {
            newCustomerClassification = new NewCTSCustomerClassificationEntity();
            IsSuccess = true;

            return this;
        }

        public INewCTSClassificationServices GetCategoryForNewCTSCustomer()
        {
            newCustomerClassification = newCTSClassificationDataAccess.GetNewCTSCustomerCategory();

            return this;
        }

        public INewCTSClassificationServices InsertVVIPForNewCustomers(RescanParametersEntity parameters)
        {
            if (newCustomerClassification != null && newCustomerClassification.VVIPClassification.Any())
            {
                var result = affectedCustomerClassificationService.InsertVVIPCategory(new VVIPCustomerInsertEntity
                {
                    CustInfo = newCustomerClassification.VVIPClassification,
                    FromAction = VVIP_FromAction.ServiceInsertNew,
                    IsAffectDownline = null,
                    QueueID = null,
                    UplineRoleID = null
                });

                IsSuccess = custClassToMainDBService.RetryPushCCToMainDB(result, Constants.RetryPushCCToExternal, parameters.ServiceName);

                var custIds = newCustomerClassification.VVIPClassification.Select(c => c.CustID).Distinct();
                antiFraudCenterServices.DeleteAFCAccounts(custIds, FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCDeleteAccountURL);
            }

            return this;
        }

        public INewCTSClassificationServices InsertPAForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassification != null && newCustomerClassification.PAClassification.Any())
            {
                var result = problemAccountServices.InsertProblemAccountLogics(newCustomerClassification.PAClassification, FunctionId.InsertNewCustJob, InsertPA_ByType.Category, false, parameters.ServiceName);

                IsSuccess = result.ErrorCode == 0;

                if (result.ErrorCode == 0)
                {
                    var custIds = result.CustomerInfo.Select(c => c.CustID);

                    IsSuccess = custClassToMainDBService.PushDataToExternal(custIds, new PushDataToExternalEntity
                    {
                        ExternalBatchSize = parameters.ExternalBatchSize,
                        FeedSoon88API = parameters.FeedSoon88API,
                        IsPushCCToSoon88 = parameters.IsPushCCToSoon88,
                        IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB,
                        IsWriteLog = parameters.IsWriteLog,
                        ServiceName = parameters.ServiceName
                    });

                    antiFraudCenterServices.InsertAFCAccounts(custIds, FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCInsertAccountURL);
                }
            }

            return this;
        }



        public INewCTSClassificationServices ResanNormalForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassification != null && newCustomerClassification.NormalClassification.Any())
            {
                parameters.CustIds = newCustomerClassification.NormalClassification.Select(c => c.CustID);

                IsSuccess = rescanCustClassificationService.RescanCustomerClassification(parameters);
            }

            return this;
        }

        public bool UpdateComplete()
        {
            if (IsSuccess)
            {
                IsSuccess = systemParameterServices.UpdateTimingExecution(newCustomerClassification.LastCTSCustID.ToString(), SystemParameters.NewCTSCustomerClassificationLastCustID);
            }

            return IsSuccess;
        }
    }
}