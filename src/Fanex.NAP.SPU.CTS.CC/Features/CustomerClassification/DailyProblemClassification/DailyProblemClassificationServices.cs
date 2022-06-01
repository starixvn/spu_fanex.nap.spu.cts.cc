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

        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(FunctionId funcId);

        IDailyProblemClassificationServices InsertProblemAccountProbationClassification(int batchSize, IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification);

        bool PushCCToExternal(PushDataToExternalEntity paras);

        bool UpdateLastScan();

        IDailyProblemClassificationServices InsertPALogic();
    }

    public class DailyProblemClassificationServices : IDailyProblemClassificationServices
    {
        private readonly IDailyProblemClassificationDataAccess dailyClassificationDataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private ProblemClassificationDailyEntity scanningCustomers;
        private IEnumerable<CustInfoEntity> problemAccounts;
        private readonly string serviceName = "ProblemAccountProbationService";

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
            problemAccounts = Enumerable.Empty<CustInfoEntity>();

            return this;
        }

        public IDailyProblemClassificationServices GetCustomerForProblemDailyScanning()
        {
            scanningCustomers = dailyClassificationDataAccess.GetCustomerForProblemDailyScanning();

            return this;
        }

        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(FunctionId funcId)
        {
            if (scanningCustomers != null && scanningCustomers.CustID != null && scanningCustomers.CustID.Any())
            {
                return problemAccountServices.GetProblemAccountLosingClassification(scanningCustomers.CustID, funcId);
            }

            return Enumerable.Empty<ProblemAccountLosingClassificationEntity>();
        }

        public IDailyProblemClassificationServices InsertProblemAccountProbationClassification(int batchSize, IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification)
        {
            if (problemClassification != null && problemClassification.Any())
            {
                var json = JsonConvert.SerializeObject(problemClassification);

                problemAccounts = dailyClassificationDataAccess.InsertProblemProbationClassification(json);
            }

            return this;
        }

        public IDailyProblemClassificationServices InsertPALogic()
        {
            if (problemAccounts != null & problemAccounts.Any())
            {
                problemAccountServices.InsertPALogics(problemAccounts, serviceName);
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