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
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownlineCustomerClassification.NewCustomerCustomerClassification.Model;
    using System;

    public interface INewCustomerClassificationServices
    {
        INewCustomerClassificationServices Start();

        INewCustomerClassificationServices GetCategoryForNewCTSCustomer();

        INewCustomerClassificationServices NewCustomerClassificationAgency(RescanParametersEntity parameters);

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
        private readonly ICustomerClassificationAgencyService customerClassificationAgencyService;
        private UInt64 lastCTSCustID;
        private NewCustomerClassificationModel newCustomerClassificationMember;
        private NewCustomerClassificationModel newCustomerClassificationAgency;
        private bool IsSuccess;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "<Pending>")]
        public NewCustomerClassificationServices(
                INewCustomerClassificationDataAccess newCTSClassificationDataAccess,
                IAffectedDownlineCustomerClassificationService affectedCustomerClassificationService,
                IProblemAccountServices problemAccountServices,
                IRescanCustClassificationService rescanCustClassificationService,
                ICustClassToMainDBService custClassToMainDBService,
                ISystemParameterServices systemParameterServices,
                IRescanNormalClassificationBySportServices rescanNormalClassificationBySportServices,
                ICustomerClassificationAgencyService customerClassificationAgencyService)
        {
            this.newCTSClassificationDataAccess = newCTSClassificationDataAccess;
            this.affectedCustomerClassificationService = affectedCustomerClassificationService;
            this.problemAccountServices = problemAccountServices;
            this.rescanCustClassificationService = rescanCustClassificationService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.systemParameterServices = systemParameterServices;
            this.rescanNormalClassificationBySportServices = rescanNormalClassificationBySportServices;
            this.customerClassificationAgencyService = customerClassificationAgencyService;
        }

        public INewCustomerClassificationServices Start()
        {
            IsSuccess = true;
            return this;
        }

        public INewCustomerClassificationServices GetCategoryForNewCTSCustomer()
        {
            var newCustomerClassification = newCTSClassificationDataAccess.GetNewCTSCustomerCategory();

            lastCTSCustID = newCustomerClassification.LastCTSCustID;

            newCustomerClassificationAgency = new NewCustomerClassificationModel
            {
                NormalClassification = newCustomerClassification.NormalClassification.Where(x => x.RoleID > (int)RoleGroupType.Member),
                PAClassification = newCustomerClassification.PAClassification.Where(x => x.RoleID > (int)RoleGroupType.Member),
                VVIPClassification = newCustomerClassification.VVIPClassification.Where(x => x.RoleID > (int)RoleGroupType.Member)
            };

            newCustomerClassificationMember = new NewCustomerClassificationModel
            {
                NormalClassification = newCustomerClassification.NormalClassification.Where(x => x.RoleID == (int)RoleGroupType.Member),
                PAClassification = newCustomerClassification.PAClassification.Where(x => x.RoleID == (int)RoleGroupType.Member),
                VVIPClassification = newCustomerClassification.VVIPClassification.Where(x => x.RoleID == (int)RoleGroupType.Member)
            };

            return this;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1067:Expressions should not be too complex", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell", "S4462:Calls to \"async\" methods should not be blocking", Justification = "<Pending>")]
        public INewCustomerClassificationServices NewCustomerClassificationAgency(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassificationAgency != null &&
                (newCustomerClassificationAgency.VVIPClassification.IsNotNullOrEmpty() ||
                newCustomerClassificationAgency.PAClassification.IsNotNullOrEmpty() ||
                newCustomerClassificationAgency.NormalClassification.IsNotNullOrEmpty()))
            {
                IsSuccess = customerClassificationAgencyService
                    .CallAPIPost(
                        newCustomerClassificationAgency,
                        parameters.InternalBatchSize, 
                        parameters.PushDataExternal.ExternalBatchSize, 
                        FromFlow.CTSServices, 
                        parameters.NewCustomerClassificationAgencyURL).Result;
            }

            return this;
        }

        public INewCustomerClassificationServices InsertVVIPForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassificationMember != null && newCustomerClassificationMember.VVIPClassification.Any())
            {
                var custInfo = new VVIPCustomerInsertEntity
                {
                    CustInfo = newCustomerClassificationMember.VVIPClassification,
                    FromFlow = FromFlow.CTSServices,
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
            if (IsSuccess && newCustomerClassificationMember != null && newCustomerClassificationMember.PAClassification.Any())
            {
                var result = problemAccountServices.InsertProblemAccountPackage(newCustomerClassificationMember.PAClassification,
                    FunctionId.InsertNewCustJob,
                    CCInputFlow.InsertPACategory,
                    parameters.IsToQueue, parameters.ServiceName);

                IsSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(result, parameters.PushDataExternal);
            }

            return this;
        }

        public INewCustomerClassificationServices ResanNormalForNewCustomers(RescanParametersEntity parameters)
        {
            if (IsSuccess && newCustomerClassificationMember != null && newCustomerClassificationMember.NormalClassification.Any())
            {
                var custIDs = newCustomerClassificationMember.NormalClassification.Select(x => x.CustID);

                IsSuccess = rescanCustClassificationService.RescanCustomerClassification(custIDs, parameters);
            }

            return this;
        }

        public INewCustomerClassificationServices ResanNormalBySportForNewCustomers()
        {
            if (!IsSuccess || newCustomerClassificationMember == null)
            {
                return this;
            }

            var custIds = new List<long>();

            if (newCustomerClassificationMember.NormalClassification.Any())
            {
                custIds.AddRange(newCustomerClassificationMember.NormalClassification.Select(x => x.CustID));
            }

            if (newCustomerClassificationMember.VVIPClassification.Any())
            {
                custIds.AddRange(newCustomerClassificationMember.VVIPClassification.Select(x => x.CustID));
            }

            if (newCustomerClassificationMember.PAClassification.Any())
            {
                custIds.AddRange(newCustomerClassificationMember.PAClassification.Select(x => x.CustID));
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
                IsSuccess = systemParameterServices.UpdateTimingExecution(lastCTSCustID.ToString(), CTS_SystemParameters.NewCTSCustomerClassificationLastCustID);
            }

            return IsSuccess;
        }
    }
}