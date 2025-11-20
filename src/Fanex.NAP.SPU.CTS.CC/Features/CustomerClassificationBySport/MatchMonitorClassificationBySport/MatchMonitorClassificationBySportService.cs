namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{  
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMatchMonitorClassificationBySportService
    {
        ClassifyGBMatchMonitorBySportModel ClassifyMatchMonitor(int batchSize);

        bool InsertProblemAccount(IEnumerable<InsertProblemAccountBySportEntity> paBySports, string serviceName, int externalBatchSize);

        void ClassifyCompletedMatchMonitor(ClassifyGBMatchMonitorBySportModel classifyGBMMModel);        
    }

    public class MatchMonitorClassificationBySportService : IMatchMonitorClassificationBySportService
    {
        private readonly IMatchMonitorClassificationBySportDataAccess matchDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountBySportService problemAccountBySportService;

        public MatchMonitorClassificationBySportService(           
            ICustClassToMainDBService custClassToMainDBService,
            IMatchMonitorClassificationBySportDataAccess matchDataAccess,
            IProblemAccountBySportService problemAccountBySportService)
        {
            this.matchDataAccess = matchDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountBySportService = problemAccountBySportService;
        }

        public ClassifyGBMatchMonitorBySportModel ClassifyMatchMonitor(int batchSize)
        {
            return matchDataAccess.ClassifyMatchMonitor(batchSize);
        }

        public bool InsertProblemAccount(IEnumerable<InsertProblemAccountBySportEntity> paBySports, string serviceName, int externalBatchSize)
        {
            var paCustBySports = problemAccountBySportService.InsertProblemAccountBySport(paBySports, CCInputFlow.BySportInsertPA, BySport_FunctionId.ClassifyGBMatchMonitor);

            return custClassToMainDBService.PushCCBySportToMainDB(paCustBySports, serviceName, externalBatchSize);
        }

        public void ClassifyCompletedMatchMonitor(ClassifyGBMatchMonitorBySportModel classifyGBMMModel)
        {
            if (classifyGBMMModel.MatchID.HasValue)
            {
                matchDataAccess.ClassifyCompletedMatchMonitor(classifyGBMMModel.MatchID.Value, classifyGBMMModel.MMDetailIDList);
            }
        }        
    }
}
