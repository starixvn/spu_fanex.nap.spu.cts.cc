namespace Fanex.NAP.SPU.CTS.Features.MatchMonitor
{
    using Fanex.NAP.SPU.CTS.Features.MatchMonitor.Entities;
    using System;
    using System.Collections.Generic;

    public interface IMatchMonitorServices
    {
        IEnumerable<TicketDetailEntity> GetMatchTrans(Int64 LastScannedTransID, int batchSize);

        IEnumerable<int> InsertMatchMonitorStaging(string matchTransJson);

        void InsertMatchMonitorRule(int tempGroupID);

        void UpdateMatchTransComplete();
    }

    public class MatchMonitorServices : IMatchMonitorServices
    {
        private readonly IMatchMonitorDataAccess matchDataAccess;

        public MatchMonitorServices(IMatchMonitorDataAccess matchDataAccess)
        {
            this.matchDataAccess = matchDataAccess;
        }

        public IEnumerable<TicketDetailEntity> GetMatchTrans(Int64 LastScannedTransID, int batchSize)
        {
            return matchDataAccess.GetMatchTrans(LastScannedTransID, batchSize);
        }

        public IEnumerable<int> InsertMatchMonitorStaging(string matchTransJson)
        {
            return matchDataAccess.InsertMatchMonitorStaging(matchTransJson);
        }

        public void InsertMatchMonitorRule(int tempGroupID)
        {
            matchDataAccess.InsertMatchMonitorRule(tempGroupID);
        }

        public void UpdateMatchTransComplete()
        {
            matchDataAccess.UpdateMatchTransComplete();
        }
    }
}