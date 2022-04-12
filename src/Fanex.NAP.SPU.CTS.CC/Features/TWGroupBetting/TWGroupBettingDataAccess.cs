namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.SPU.CTS.Shares;

    public interface ITWGroupBettingDataAccess
    {
        Int64 ScanTicket(DateTime fromTime, DateTime toTime, int step, Int64 lastTransID);

        void CountGroupBettingTicket();

        void CountGroupBettingTicketForDB062();

        DateTime CountTotalTicket(DateTime lastUpdatedTotalTicketTime);

        IEnumerable<TWGroupBettingRule> GetAllRules();

        void SyncGroupBettingTicketToDB062(IEnumerable<TWGroupBettingTicket> gbTickets);

    }


    public class TWGroupBettingDataAccess : ITWGroupBettingDataAccess
    {
        private readonly IDynamicRepository repository;

        public TWGroupBettingDataAccess()
        {
            repository = new DynamicRepository();
        }

        public Int64 ScanTicket(DateTime fromTime, DateTime toTime, int step, Int64 lastTransID)
        {
            var criteria = new TWGroupBetting_ScanTicketCriteria
            {
                FromTime = fromTime,
                ToTime = toTime,
                Step = step,
                LastTransID = lastTransID
            };

            repository.Fetch<TWGroupBettingTicket>(criteria);
            return criteria.MaxTransID;
        }

        public void CountGroupBettingTicket()
        {
            var criteria = new TWGroupBetting_CountGBTicketCriteria();
            repository.Execute(criteria);
        }
        
        public void CountGroupBettingTicketForDB062()
        {
            var criteria = new TWGroupBetting_CountGBTicketDB062Criteria();
            repository.Execute(criteria);
        }

        public DateTime CountTotalTicket(DateTime lastUpdatedTotalTicketTime)
        {
            var criteria = new TWGroupBetting_UpdateTotalTicketCriteria
            {
                LastScannedTime = lastUpdatedTotalTicketTime,
                LastestScannedTime = lastUpdatedTotalTicketTime
            };

            repository.Execute(criteria);
            return criteria.LastestScannedTime;
        }
        
        public IEnumerable<TWGroupBettingRule> GetAllRules()
        {
            var criteria = new TWGroupBetting_GetRuleCriteria();

            return repository.Fetch<TWGroupBettingRule>(criteria);
        }

        public void SyncGroupBettingTicketToDB062(IEnumerable<TWGroupBettingTicket> gbTickets)
        {
            var criteria = new TWGroupBetting_SyncDataGroupBettingTicket 
            { 
                GBTicketXML = gbTickets.ToXmlString() 
            };

            repository.Execute(criteria);
        }
    }
}
