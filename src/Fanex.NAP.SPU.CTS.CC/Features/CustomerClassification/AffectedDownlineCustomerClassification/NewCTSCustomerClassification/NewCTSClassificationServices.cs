namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
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
        private readonly IAffectedCustomerClassificationService affectedCustomerClassificationService;
        private readonly IDailyProblemClassificationServices dailyProblemClassificationServices;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISystemParameterServices systemParameterServices;
        private NewCTSCustomerClassificationEntity newCustomerClassification;
        private bool IsSuccess;

        public NewCTSClassificationServices(
                INewCTSClassificationDataAccess newCTSClassificationDataAccess,
                IAffectedCustomerClassificationService affectedCustomerClassificationService,
                IDailyProblemClassificationServices dailyProblemClassificationServices,
                IProblemAccountServices problemAccountServices,
                IRescanCustClassificationService rescanCustClassificationService,
                IAntiFraudCenterServices antiFraudCenterServices,
                ISystemParameterServices systemParameterServices)
        {
            this.newCTSClassificationDataAccess = newCTSClassificationDataAccess;
            this.affectedCustomerClassificationService = affectedCustomerClassificationService;
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;
            this.problemAccountServices = problemAccountServices;
            this.rescanCustClassificationService = rescanCustClassificationService;
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

        public INewCTSClassificationServices InsertPAForNewCustomers(RescanParametersEntity parameters)
        {
            if (newCustomerClassification != null && newCustomerClassification.PAClassification.Any())
            {
                var paLosing = dailyProblemClassificationServices.GetProblemAccountLosingClassification(newCustomerClassification.PAClassification.Select(c => c.CustID), FunctionId.InsertNewCustJob);

                var result = problemAccountServices.InsertProblemAccountLogics(newCustomerClassification.PAClassification, paLosing, InsertPA_ByType.Category, false, parameters.ServiceName);

                IsSuccess = result.ErrorCode == 0;

                if (result.ErrorCode == 0)
                {
                    parameters.CustIds = result.CustomerInfo.Select(c => c.CustID);

                    rescanCustClassificationService.PushExternalData(parameters);
                }
            }

            return this;
        }

        public INewCTSClassificationServices InsertVVIPForNewCustomers(RescanParametersEntity parameters)
        {
            if (newCustomerClassification != null && newCustomerClassification.VVIPClassification.Any())
            {
                affectedCustomerClassificationService.InsertVVIP(new VVIPCustomerInsertEntity
                {
                    CustInfo = newCustomerClassification.VVIPClassification,
                    FromAction = VVIP_FromAction.ServiceInsertNew,
                    IsAffectDownline = null,
                    QueueID = null,
                    UplineRoleID = null
                });

                var custIds = newCustomerClassification.VVIPClassification.Select(c => c.CustID).Distinct();

                antiFraudCenterServices.DeleteAFCAccounts(custIds, FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCDeleteAccountURL);
            }

            return this;
        }

        public INewCTSClassificationServices ResanNormalForNewCustomers(RescanParametersEntity parameters)
        {
            if (newCustomerClassification != null && newCustomerClassification.NormalClassification.Any())
            {
                rescanCustClassificationService.RescanCustomerClassification(parameters);
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