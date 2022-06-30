namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{   
    public interface IMatchMonitorClassificationService
    {
        ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor();

        void ClassifyGBMatchMonitorCompleted(string mmDetailIDs);
    }

    public class MatchMonitorClassificationService : IMatchMonitorClassificationService
    {
        private readonly IMatchMonitorClassificationDataAccess matchDataAccess;

        public ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor()
        {
            return matchDataAccess.ClassifyGBMatchMonitor();
        }

        public void ClassifyGBMatchMonitorCompleted(string mmDetailIDs)
        {
            matchDataAccess.ClassifyGBMatchMonitorCompleted(mmDetailIDs);
        }
    }
}
