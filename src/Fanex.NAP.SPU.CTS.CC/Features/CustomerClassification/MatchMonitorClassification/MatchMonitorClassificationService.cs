namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using System.Linq;

    public interface IMatchMonitorClassificationService
    {
        ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor();

        void ClassifyGBMatchMonitorCompleted(ClassifyGBMatchMonitorModel classifyGBMMModel);
    }

    public class MatchMonitorClassificationService : IMatchMonitorClassificationService
    {
        private readonly IMatchMonitorClassificationDataAccess matchDataAccess;

        public MatchMonitorClassificationService(IMatchMonitorClassificationDataAccess matchDataAccess)
        {
            this.matchDataAccess = matchDataAccess;
        }

        public ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor()
        {
            return matchDataAccess.ClassifyGBMatchMonitor();
        }

        public void ClassifyGBMatchMonitorCompleted(ClassifyGBMatchMonitorModel classifyGBMMModel)
        {
            if (classifyGBMMModel.MatchID.HasValue)
            {                
                matchDataAccess.ClassifyGBMatchMonitorCompleted(classifyGBMMModel.MatchID.Value, classifyGBMMModel.MMDetailIDList);
            }            
        }
    }
}
