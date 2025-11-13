namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{  
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMatchMonitorClassificationBySportService
    {
        ClassifyGBMatchMonitorBySportModel ClassifyMatchMonitor();

        void ClassifyMatchMonitorCompleted(ClassifyGBMatchMonitorBySportModel classifyGBMMModel);

        bool InsertProblemAccount(IEnumerable<InsertProblemAccountBySportEntity> paBySports, PushDataToExternalEntity paras);
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

        public ClassifyGBMatchMonitorBySportModel ClassifyMatchMonitor()
        {
            return matchDataAccess.ClassifyMatchMonitor();
        }

        public void ClassifyMatchMonitorCompleted(ClassifyGBMatchMonitorBySportModel classifyGBMMModel)
        {
            if (classifyGBMMModel.MatchID.HasValue)
            {
                matchDataAccess.ClassifyMatchMonitorCompleted(classifyGBMMModel.MatchID.Value, classifyGBMMModel.MMDetailIDList);
            }
        }

        public bool InsertProblemAccount(IEnumerable<InsertProblemAccountBySportEntity> paBySports, PushDataToExternalEntity paras)
        {                        
            var paCustIDs = problemAccountBySportService.InsertProblemAccountBySport(paBySports, CCInputFlow.BySportInsertPA);

            //custClassToMainDBService.PushExternalProblemAcountPackage(paCustIDs.Distinct(), paras);

            return true;
        }
    }
}
