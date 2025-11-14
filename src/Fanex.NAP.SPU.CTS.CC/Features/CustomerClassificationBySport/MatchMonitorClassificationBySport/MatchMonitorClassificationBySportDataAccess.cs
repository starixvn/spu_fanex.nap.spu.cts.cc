namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    using Fanex.Data.Repository;
    using System.Linq;

    public interface IMatchMonitorClassificationBySportDataAccess
    {
        ClassifyGBMatchMonitorBySportModel ClassifyMatchMonitor(int batchSize);

        void ClassifyCompletedMatchMonitor(int matchID, string mmDetailIDs);
    }

    public class MatchMonitorClassificationBySportDataAccess : IMatchMonitorClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public MatchMonitorClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ClassifyGBMatchMonitorBySportModel ClassifyMatchMonitor(int batchSize)
        {
            var criteria = new ClassifyMatchMonitorBySportCriteria { BatchSize = batchSize };

            var (custInfos, MMDetailIDs, matchIDs) = repository.FetchMultiple<ClassifyGBMMCustInfosBySportEntity, string, int?>(criteria);

            return new ClassifyGBMatchMonitorBySportModel
            {
                CustInfos = custInfos,
                MMDetailIDList = MMDetailIDs != null ? MMDetailIDs.FirstOrDefault() : null,
                MatchID = matchIDs != null ? matchIDs.FirstOrDefault() : null
            };
        }

        public void ClassifyCompletedMatchMonitor(int matchID, string mmDetailIDs)
        {
            var criteria = new ClassifyCompletedMatchMonitorBySportCriteria
            {
                MatchID = matchID,
                MMDetailsJson = mmDetailIDs
            };

            repository.Execute(criteria);
        }
    }
}
