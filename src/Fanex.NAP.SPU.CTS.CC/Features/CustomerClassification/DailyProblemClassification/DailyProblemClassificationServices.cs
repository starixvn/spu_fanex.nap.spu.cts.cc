namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyProblemClassificationServices
    {
        IDailyProblemClassificationServices Start();

        IDailyProblemClassificationServices Reset();

        IDailyProblemClassificationServices GetPADailyScanFromQueue();

        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(FunctionId funcId);

        IDailyProblemClassificationServices InsertProblemAccountProbationClassification(int batchSize, IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification);

        bool PushCCToExternal(PushDataToExternalEntity paras);

        bool UpdateComplete();

        IDailyProblemClassificationServices InsertPALogic();
    }

    public class DailyProblemClassificationServices : IDailyProblemClassificationServices
    {
        private readonly IDailyProblemClassificationDataAccess dailyClassificationDataAccess;       
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private PADailyScanFromQueueModel paDailyScanFromQueue;
        private IEnumerable<InsertPAProbationResult> problemAccounts;
        private readonly string serviceName = "ProblemAccountProbationService";

        public DailyProblemClassificationServices(
            IDailyProblemClassificationDataAccess dailyClassificationDataAccess,            
            IProblemAccountServices problemAccountServices,           
            ICustClassToMainDBService custClassToMainDBService)
        {
            this.dailyClassificationDataAccess = dailyClassificationDataAccess;
            this.problemAccountServices = problemAccountServices;          
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public IDailyProblemClassificationServices Start()
        {
            paDailyScanFromQueue = new PADailyScanFromQueueModel();

            return this;
        }

        public IDailyProblemClassificationServices Reset()
        {
            problemAccounts = Enumerable.Empty<InsertPAProbationResult>();

            return this;
        }

        public IDailyProblemClassificationServices GetPADailyScanFromQueue()
        {
            paDailyScanFromQueue = dailyClassificationDataAccess.GetPADailyScanFromQueue();

            return this;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1905:Redundant casts should not be used", Justification = "<Pending>")]
        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(FunctionId funcId)
        {
            if (paDailyScanFromQueue.PADailyScanCustIDs != null && paDailyScanFromQueue.PADailyScanCustIDs.Any())
            {
                return problemAccountServices.GetProblemAccountLosingClassification(paDailyScanFromQueue.PADailyScanCustIDs.Select(x=>(long)x), funcId);
            }

            return Enumerable.Empty<ProblemAccountLosingClassificationEntity>();
        }

        public IDailyProblemClassificationServices InsertProblemAccountProbationClassification(int batchSize, IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification)
        {
            if (problemClassification != null && problemClassification.Any())
            {                
                var pasJson = JsonConvert.SerializeObject(problemClassification);

                problemAccounts = dailyClassificationDataAccess.InsertProblemProbationClassification(pasJson);
            }

            return this;
        }       

        public IDailyProblemClassificationServices InsertPALogic()
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var probAccountToInsertLogic = problemAccounts.Where(c => !c.IsRobot).Select(c => new CustInfoEntity { CTSCustID = c.CTSCustID, CustID = c.CustID });
                problemAccountServices.InsertPALogics(probAccountToInsertLogic, serviceName);

                var robotToInsertLogic = problemAccounts.Where(c => c.IsRobot).Select(c => new CustInfoEntity { CTSCustID = c.CTSCustID, CustID = c.CustID });
                problemAccountServices.InsertRobotLogics(robotToInsertLogic);
            }

            return this;
        }

        public bool PushCCToExternal(PushDataToExternalEntity paras)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var custIds = problemAccounts.Select(c => c.CustID).Distinct();

                return custClassToMainDBService.PushExternalPackageByCustId(custIds, paras);
            }

            return true;
        }

        public bool UpdateComplete()
        {
            string custIdToString = string.Empty;

            if (paDailyScanFromQueue.PADailyScanCustIDs != null && paDailyScanFromQueue.PADailyScanCustIDs.Any())
            {
                custIdToString = string.Join(",", paDailyScanFromQueue.PADailyScanCustIDs.Distinct());
            }

            dailyClassificationDataAccess.CompletePADailyScan(custIdToString);

            return (paDailyScanFromQueue.PADailyScanCustIDs != null && paDailyScanFromQueue.PADailyScanCustIDs.Any());
        }
    }
}