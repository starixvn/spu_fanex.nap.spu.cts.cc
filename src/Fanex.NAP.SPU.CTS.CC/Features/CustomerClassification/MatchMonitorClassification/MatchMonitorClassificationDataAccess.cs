namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using Fanex.Data.Repository;
    using System.Linq;

    public interface IMatchMonitorClassificationDataAccess
    {
        ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor();

        void ClassifyGBMatchMonitorCompleted(int matchID, string mmDetailIDs);
    }

    public class MatchMonitorClassificationDataAccess : IMatchMonitorClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public MatchMonitorClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor()
        {
            var criteria = new ClassifyGBMatchMonitorCriteria();

            var (custInfos, mmDetailIDs, matchIDs) = repository.FetchMultiple<CustInfosEntity, int, int>(criteria);

            return new ClassifyGBMatchMonitorModel
            {
                CustInfos = custInfos,
                MMDetailIDs = mmDetailIDs,
                MatchID = matchIDs != null ? matchIDs.FirstOrDefault(): null
            };
        }

        public void ClassifyGBMatchMonitorCompleted(int matchID, string mmDetailIDs)
        {
            var criteria = new ClassifyGBMatchMonitorCompletedCriteria
            {
                MatchID = matchID,
                MMDetailsJson = mmDetailIDs
            };

            repository.Execute(criteria);
        }
    }
}
