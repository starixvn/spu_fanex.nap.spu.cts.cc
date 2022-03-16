namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using System;
    using System.Collections.Generic;

    public interface ITWGroupBettingDataAccess
    {
        void ScanTicket(DateTime fromTime, DateTime toTime, int step);

        void CountGroupBettingTicket();

        DateTime CountTotalTicket(DateTime lastUpdatedTotalTicketTime);

        IEnumerable<TWGroupBettingRule> GetAllRules();

    }


    public class TWGroupBettingDataAccess : ITWGroupBettingDataAccess
    {
        private readonly IDynamicRepository repository;

        public TWGroupBettingDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void ScanTicket(DateTime fromTime, DateTime toTime, int step)
        {
            var criteria = new TWGroupBetting_ScanTicketCriteria
            {
                FromTime = fromTime,
                ToTime = toTime,
                Step = step
            };

            repository.Execute(criteria);
        }

        public void CountGroupBettingTicket()
        {
            var criteria = new TWGroupBetting_CountGBTicketCriteria();
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
    }
}
