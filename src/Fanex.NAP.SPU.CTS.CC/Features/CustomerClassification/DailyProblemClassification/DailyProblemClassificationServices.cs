namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyProblemClassificationServices
    {
        IDailyProblemClassificationServices Start();

        IDailyProblemClassificationServices Reset();

        IDailyProblemClassificationServices GetCustomerForProblemDailyScanning();

        IEnumerable<ProblemAccountClassificationEntity> GetProblemAccountClassification(FunctionId funcId);

        IDailyProblemClassificationServices InsertProblemAccountProbationClassification(int batchSize, IEnumerable<ProblemAccountClassificationEntity> problemClassification);

        IDailyProblemClassificationServices CheckNewPAAssociationCustomerForRollback();

        bool PushCCToExternal(PushDataToExternalEntity paras);

        bool UpdateLastScan();
    }

    public class DailyProblemClassificationServices : IDailyProblemClassificationServices
    {
        private readonly IDailyProblemClassificationDataAccess dailyClassificationDataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private ProblemClassificationDailyEntity scanningCustomers;
        private IEnumerable<CheckPACustomerAssociationForRollbackEntity> problemAccounts;

        public DailyProblemClassificationServices(
            IDailyProblemClassificationDataAccess dailyClassificationDataAccess,
            ISystemParameterServices systemParameterService,
            IProblemAccountServices problemAccountServices,
            ICustClassToMainDBService custClassToMainDBService)
        {
            this.dailyClassificationDataAccess = dailyClassificationDataAccess;
            this.systemParameterService = systemParameterService;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public IDailyProblemClassificationServices Start()
        {
            scanningCustomers = new ProblemClassificationDailyEntity();

            return this;
        }

        public IDailyProblemClassificationServices Reset()
        {
            problemAccounts = Enumerable.Empty<CheckPACustomerAssociationForRollbackEntity>();

            return this;
        }

        public IDailyProblemClassificationServices GetCustomerForProblemDailyScanning()
        {
            scanningCustomers = dailyClassificationDataAccess.GetCustomerForProblemDailyScanning();

            return this;
        }

        public IEnumerable<ProblemAccountClassificationEntity> GetProblemAccountClassification(FunctionId funcId)
        {
            if (scanningCustomers != null && scanningCustomers.CustID != null && scanningCustomers.CustID.Any())
            {
                var custIds = string.Join(",", scanningCustomers.CustID);

                return dailyClassificationDataAccess.GetProblemClassifiction(custIds, funcId);
            }

            return Enumerable.Empty<ProblemAccountClassificationEntity>();
        }

        public IDailyProblemClassificationServices InsertProblemAccountProbationClassification(int batchSize, IEnumerable<ProblemAccountClassificationEntity> problemClassification)
        {
            if (problemClassification != null && problemClassification.Any())
            {
                var json = JsonConvert.SerializeObject(problemClassification);

                problemAccounts = dailyClassificationDataAccess.InsertProblemProbationClassification(json);
            }

            return this;
        }

        public IDailyProblemClassificationServices CheckNewPAAssociationCustomerForRollback()
        {
            if (problemAccounts != null & problemAccounts.Any())
            {
                problemAccountServices.CheckNewPAAssociationCustomerForRollback(problemAccounts);
            }

            return this;
        }

        public bool PushCCToExternal(PushDataToExternalEntity paras)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var custIds = problemAccounts.Select(c => c.CustID).Distinct();

                return custClassToMainDBService.PushDataToExternal(custIds, paras);
            }

            return true;
        }

        public bool UpdateLastScan()
        {
            if (scanningCustomers != null)
            {
                systemParameterService.UpdateTimingExecution(scanningCustomers.op_LastCustID.ToString(), SystemParameters.DailyProblemClassificationLastCustID);
                systemParameterService.UpdateTimingExecution(scanningCustomers.op_LastCategoryID.ToString(), SystemParameters.DailyProblemClassificationLastCategoryID);

                return (scanningCustomers.CustID != null && scanningCustomers.CustID.Any());
            }

            return false;
        }
    }
}