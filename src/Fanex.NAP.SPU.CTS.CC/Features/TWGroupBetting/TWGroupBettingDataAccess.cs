namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;

    public interface ITWGroupBettingDataAccess
    {
        TWGroupBetting_ScanTicketCriteria ScanTicket(TWGroupBetting_ScanTicketCriteria criteria);

        void CountGroupBettingTicket();

        void CountGroupBettingTicketForDB062();

        long CountTotalTicket(int batchSize, long LastSequenceId);

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

        public TWGroupBetting_ScanTicketCriteria ScanTicket(TWGroupBetting_ScanTicketCriteria criteria)
        {
            repository.Fetch<TWGroupBettingTicket>(criteria);
            return criteria;
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

        public long CountTotalTicket(int batchSize, long lastSequenceId)
        {
            var criteria = new TWGroupBetting_UpdateTotalTicketCriteria(false)
            {
                BatchSize = batchSize,
                LastSequenceId = lastSequenceId
            };

            repository.Execute(criteria);

            return criteria.LastSequenceId_New;
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