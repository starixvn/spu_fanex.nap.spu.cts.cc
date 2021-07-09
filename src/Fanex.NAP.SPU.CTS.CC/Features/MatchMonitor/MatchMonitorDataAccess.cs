namespace Fanex.NAP.SPU.CTS.Features.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.MatchMonitor.Criteria;
    using Fanex.NAP.SPU.CTS.Features.MatchMonitor.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IMatchMonitorDataAccess
    {
        IEnumerable<TicketDetailEntity> GetMatchTrans(Int64 LastScannedTransID, int batchSize);

        IEnumerable<int> InsertMatchMonitorStaging(string matchTransJson);

        void InsertMatchMonitorRule(int tempGroupID);

        void UpdateMatchTransComplete();
    }

    public class MatchMonitorDataAccess : IMatchMonitorDataAccess
    {
        readonly IDynamicRepository repository;

        public MatchMonitorDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<TicketDetailEntity> GetMatchTrans(Int64 LastScannedTransID, int batchSize)
        {
            var criteria = new MatchTransGetCriteria
            {
                QueryType = (int)Constants.MatchMonitorQueryTypes.Service,
                LastScannedTransID = LastScannedTransID,
                BatchSize = batchSize,
                EventDate = DateTime.Now
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<TicketDetailEntity>(criteria).ToList();
            }
        }

        public IEnumerable<int> InsertMatchMonitorStaging(string matchTransJson)
        {
            var criteria = new MatchMonitorStagingInsertCriteria
            {
                MatchTransJson = matchTransJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<int>(criteria).ToList();
            }
        }

        public void InsertMatchMonitorRule(int tempGroupID)
        {
            var criteria = new MatchMonitorRuleInsertCriteria
            {
                TempGroupID = tempGroupID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateMatchTransComplete()
        {
            var criteria = new MatchTransUpdateCompleteCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }
    }
}
