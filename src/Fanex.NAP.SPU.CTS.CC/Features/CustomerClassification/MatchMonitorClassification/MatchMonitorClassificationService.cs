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

        public ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor()
        {
            return matchDataAccess.ClassifyGBMatchMonitor();
        }

        public void ClassifyGBMatchMonitorCompleted(ClassifyGBMatchMonitorModel classifyGBMMModel)
        {
            if (classifyGBMMModel.MatchID.HasValue)
            {
                var mmDetailIDList = string.Join(",", classifyGBMMModel.MMDetailIDs.Select(x => x));

                matchDataAccess.ClassifyGBMatchMonitorCompleted(classifyGBMMModel.MatchID.Value, mmDetailIDList);
            }            
        }
    }
}
