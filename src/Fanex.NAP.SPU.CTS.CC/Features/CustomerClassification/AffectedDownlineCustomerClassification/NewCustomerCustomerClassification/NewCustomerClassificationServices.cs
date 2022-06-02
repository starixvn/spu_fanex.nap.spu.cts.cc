namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INewCustomerClassificationServices
    {
        INewCustomerClassificationServices Start();

        INewCustomerClassificationServices GetCategoryForNewCTSCustomer();

        INewCustomerClassificationServices InsertPAForNewCustomers(RescanParametersEntity parameters);

        INewCustomerClassificationServices InsertVVIPForNewCustomers(RescanParametersEntity parameters);

        INewCustomerClassificationServices ResanNormalForNewCustomers(RescanParametersEntity parameters);

        bool UpdateComplete();
    }

    public class NewCustomerClassificationServices : INewCustomerClassificationServices
    {
        private readonly INewCustomerClassificationDataAccess newCTSClassificationDataAccess;
        private readonly IAffectedDownlineCustomerClassificationService affectedCustomerClassificationService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISystemParameterServices systemParameterServices;
        private NewCustomerClassificationEntity newCustomerClassification;
        private bool IsSuccess;

        public NewCustomerClassificationServices(
                INewCustomerClassificationDataAccess newCTSClassificationDataAccess,
                IAffectedDownlineCustomerClassificationService affectedCustomerClassificationService,
                IProblemAccountServices problemAccountServices,
                IRescanCustClassificationService rescanCustClassificationService,
                ICustClassToMainDBService custClassToMainDBService,
                ISystemParameterServices systemParameterServices)
        {
            this.newCTSClassificationDataAccess = newCTSClassificationDataAccess;
            this.affectedCustomerClassificationService = affectedCustomerClassificationService;
            this.problemAccountServices = problemAccountServices;
            this.rescanCustClassificationService = rescanCustClassificationService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.systemParameterServices = systemParameterServices;
        }

        public INewCustomerClassificationServices Start()
        {
            newCustomerClassification = new NewCustomerClassificationEntity();
            IsSuccess = true;
            return this;
        }

        public INewCustomerClassificationServices GetCategoryForNewCTSCustomer()
        {
            newCustomerClassification = newCTSClassificationDataAccess.GetNewCTSCustomerCategory();
            return this;
        }

        public INewCustomerClassificationServices InsertVVIPForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassification != null && newCustomerClassification.VVIPClassification.Any())
            {
                var custInfo = new VVIPCustomerInsertEntity
                {
                    CustInfo = newCustomerClassification.VVIPClassification,
                    FromAction = VVIP_FromAction.ServiceInsertNew,
                    IsAffectDownline = null,
                    QueueID = null,
                    UplineRoleID = null
                };

                IsSuccess = affectedCustomerClassificationService.InsertVVIPForDownlinePackage(parameters, custInfo);
            }

            return this;
        }

        public INewCustomerClassificationServices InsertPAForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassification != null && newCustomerClassification.PAClassification.Any())
            {
                var result = problemAccountServices.InsertProblemAccountPackage(newCustomerClassification.PAClassification, FunctionId.InsertNewCustJob, InsertPA_ByType.Category, false, parameters.ServiceName);
                IsSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(result, parameters);
            }

            return this;
        }

        public INewCustomerClassificationServices ResanNormalForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassification != null && newCustomerClassification.NormalClassification.Any())
            {
                parameters.CustIds = newCustomerClassification.NormalClassification.Select(c => c.CustID).Distinct();

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