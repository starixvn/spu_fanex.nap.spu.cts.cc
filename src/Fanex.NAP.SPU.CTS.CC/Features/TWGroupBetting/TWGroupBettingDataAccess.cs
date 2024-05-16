namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using System.Collections.Generic;

    public interface ITWGroupBettingDataAccess
    {
        TWGroupBetting_ScanTicketCriteria ScanTicket(TWGroupBetting_ScanTicketCriteria criteria);

        long UpdateTotalTWGBTicket(int batchSize, long LastSequenceId);

        IEnumerable<TWGroupBettingRule> GetAllRules();
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

        public long UpdateTotalTWGBTicket(int batchSize, long lastSequenceId)
        {
            var criteria = new TWGroupBetting_UpdateTotalTicketCriteria()
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
    }
}