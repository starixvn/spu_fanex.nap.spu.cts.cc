namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface IMatchMonitorClassificationDataAccess
    {
        IEnumerable<int> GetMatchMonitor();

        IEnumerable<MatchesInfosEntity> GetMatchInfosByIDs(string matchIDs);

        void UpdateStatusMatchMonitor(IEnumerable<MatchStatusModel> matchStatuses);

        ClassifyGBMatchMonitorModel ClassifyMatchMonitor();

        void ClassifyMatchMonitorCompleted(int matchID, string mmDetailIDs);        
    }

    public class MatchMonitorClassificationDataAccess : IMatchMonitorClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public MatchMonitorClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<int> GetMatchMonitor()
        {
            var criteria = new GetMatchMonitorCriteria();

            return repository.Fetch<int>(criteria);
        }

        public IEnumerable<MatchesInfosEntity> GetMatchInfosByIDs(string matchIDs)
        {
            var criteria = new GetMatchesInfoCriteria
            {
                MatchIds = matchIDs
            };

            return repository.Fetch<MatchesInfosEntity>(criteria);
        }

        public void UpdateStatusMatchMonitor(IEnumerable<MatchStatusModel> matchStatuses)
        {
            var criteria = new UpdateStatusMatchMonitorCriteria
            {
                MatchStatusInfos = JsonConvert.SerializeObject(matchStatuses)
            };

            repository.Execute(criteria);
        }

        public ClassifyGBMatchMonitorModel ClassifyMatchMonitor()
        {
            var criteria = new ClassifyMatchMonitorCriteria();

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
            var criteria = new ClassifyMatchMonitorCompletedCriteria
            {
                MatchID = matchID,
                MMDetailsJson = mmDetailIDs
            };

            repository.Execute(criteria);
        }
    }
}
