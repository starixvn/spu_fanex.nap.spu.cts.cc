namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RescanNormalClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INewCustomerClassificationServices
    {
        INewCustomerClassificationServices Start();

        INewCustomerClassificationServices GetCategoryForNewCTSCustomer();

        INewCustomerClassificationServices InsertPAForNewCustomers(RescanParametersEntity parameters);

        INewCustomerClassificationServices InsertVVIPForNewCustomers(RescanParametersEntity parameters);

        INewCustomerClassificationServices ResanNormalForNewCustomers(RescanParametersEntity parameters);

        INewCustomerClassificationServices ResanNormalBySportForNewCustomers();

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
        private readonly IRescanNormalClassificationBySportServices rescanNormalClassificationBySportServices;
        private NewCustomerClassificationEntity newCustomerClassification;
        private bool IsSuccess;

        public NewCustomerClassificationServices(
                INewCustomerClassificationDataAccess newCTSClassificationDataAccess,
                IAffectedDownlineCustomerClassificationService affectedCustomerClassificationService,
                IProblemAccountServices problemAccountServices,
                IRescanCustClassificationService rescanCustClassificationService,
                ICustClassToMainDBService custClassToMainDBService,
                ISystemParameterServices systemParameterServices,
                IRescanNormalClassificationBySportServices rescanNormalClassificationBySportServices)
        {
            this.newCTSClassificationDataAccess = newCTSClassificationDataAccess;
            this.affectedCustomerClassificationService = affectedCustomerClassificationService;
            this.problemAccountServices = problemAccountServices;
            this.rescanCustClassificationService = rescanCustClassificationService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.systemParameterServices = systemParameterServices;
            this.rescanNormalClassificationBySportServices = rescanNormalClassificationBySportServices;
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
                    FromAction = FromAction.ServiceInsertNew,
                    IsAffectDownline = false,
                    QueueID = null,
                    UplineRoleID = null
                };

                IsSuccess = affectedCustomerClassificationService.InsertVVIP(parameters, custInfo);
            }

            return this;
        }

        public INewCustomerClassificationServices InsertPAForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassification != null && newCustomerClassification.PAClassification.Any())
            {
                var result = problemAccountServices.InsertProblemAccountPackage(newCustomerClassification.PAClassification, FunctionId.InsertNewCustJob, InsertPA_ByType.Category, parameters.IsToQueue, parameters.ServiceName);
                IsSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(result, parameters.PushDataExternal);
            }

            return this;
        }

        public INewCustomerClassificationServices ResanNormalForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassification != null && newCustomerClassification.NormalClassification.Any())
            {                
                var cusIDs = newCustomerClassification.NormalClassification.Select(x=> x.CustID);

                IsSuccess = rescanCustClassificationService.RescanCustomerClassification(cusIDs, parameters);
            }

            return this;
        }

        public INewCustomerClassificationServices ResanNormalBySportForNewCustomers()
        {
            if (!IsSuccess || newCustomerClassification == null)
            {
                return this;
            }

            var custIds = new List<long>();

            if (newCustomerClassification.NormalClassification.Any())
            {
                custIds.AddRange(newCustomerClassification.NormalClassification.Select(x => x.CustID));
            }

            if (newCustomerClassification.VVIPClassification.Any())
            {
                custIds.AddRange(newCustomerClassification.VVIPClassification.Select(x => x.CustID));
            }

            if (newCustomerClassification.PAClassification.Any())
            {
                custIds.AddRange(newCustomerClassification.PAClassification.Select(x => x.CustID));
            }

            if (custIds.Any())
            {
                rescanNormalClassificationBySportServices.PreprocessRescanNormalBySportClassification(custIds.Distinct(), BySport_FunctionId.InsertNewCustJob);
            }

            return this;
        }

        public bool UpdateComplete()
        {
            if (IsSuccess)
            {
                IsSuccess = systemParameterServices.UpdateTimingExecution(newCustomerClassification.LastCTSCustID.ToString(), CTS_SystemParameters.NewCTSCustomerClassificationLastCustID);
            }

            return IsSuccess;
        }
    }
}