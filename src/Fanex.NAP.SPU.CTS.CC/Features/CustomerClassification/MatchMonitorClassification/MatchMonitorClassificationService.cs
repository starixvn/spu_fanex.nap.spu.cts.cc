namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IMatchMonitorClassificationService
    {
        IEnumerable<int> GetMatchMonitor();

        IEnumerable<MatchesInfosEntity> GetMatchInfosByIDs(string matchIDs);

        void UpdateStatusMatchMonitor(IEnumerable<MatchStatusModel> matchStatuses);

        ClassifyGBMatchMonitorModel ClassifyMatchMonitor();

        void ClassifyMatchMonitorCompleted(ClassifyGBMatchMonitorModel classifyGBMMModel);
    }

    public class MatchMonitorClassificationService : IMatchMonitorClassificationService
    {
        private readonly IMatchMonitorClassificationDataAccess matchDataAccess;

        public MatchMonitorClassificationService(IMatchMonitorClassificationDataAccess matchDataAccess)
        {
            this.matchDataAccess = matchDataAccess;
        }

        public IEnumerable<int> GetMatchMonitor()
        {
            return matchDataAccess.GetMatchMonitor();
        }

        public IEnumerable<MatchesInfosEntity> GetMatchInfosByIDs(string matchIDs)
        {
            if (!string.IsNullOrEmpty(matchIDs))
            {
                return matchDataAccess.GetMatchInfosByIDs(matchIDs);
            }

            return Enumerable.Empty<MatchesInfosEntity>();
        }

        public void UpdateStatusMatchMonitor(IEnumerable<MatchStatusModel> matchStatuses)
        {
            matchDataAccess.UpdateStatusMatchMonitor(matchStatuses);
        }

        public ClassifyGBMatchMonitorModel ClassifyMatchMonitor()
        {
            return matchDataAccess.ClassifyMatchMonitor();
        }

        public void ClassifyMatchMonitorCompleted(ClassifyGBMatchMonitorModel classifyGBMMModel)
        {
            if (classifyGBMMModel.MatchID.HasValue)
            {                
                matchDataAccess.ClassifyMatchMonitorCompleted(classifyGBMMModel.MatchID.Value, classifyGBMMModel.MMDetailIDList);
            }            
        }
    }
}
