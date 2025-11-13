namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.Data.Repository;
    using System.Linq;

    public interface IMatchMonitorClassificationBySportDataAccess
    {
        ClassifyGBMatchMonitorModel ClassifyMatchMonitor();

        void ClassifyMatchMonitorCompleted(int matchID, string mmDetailIDs);
    }

    public class MatchMonitorClassificationBySportDataAccess : IMatchMonitorClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public MatchMonitorClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ClassifyGBMatchMonitorModel ClassifyMatchMonitor()
        {
            var criteria = new ClassifyMatchMonitorBySportCriteria();

            var (custInfos, MMDetailIDs, matchIDs) = repository.FetchMultiple<ClassifyGBMMCustInfosEntity, string, int?>(criteria);

            return new ClassifyGBMatchMonitorModel
            {
                CustInfos = custInfos,
                MMDetailIDList = MMDetailIDs != null ? MMDetailIDs.FirstOrDefault() : null,
                MatchID = matchIDs != null ? matchIDs.FirstOrDefault() : null
            };
        }

        public void ClassifyMatchMonitorCompleted(int matchID, string mmDetailIDs)
        {
            var criteria = new ClassifyMatchMonitorBySportCompletedCriteria
            {
                MatchID = matchID,
                MMDetailsJson = mmDetailIDs
            };

            repository.Execute(criteria);
        }
    }
}
