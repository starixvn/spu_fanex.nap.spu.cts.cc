namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using System;
    using System.Collections.Generic;

    public interface ITWInitialGroupBettingDataAccess
    {
        TWInitialGBGetChangesModel GetChanges();

        void UpdateNextScannedTime(DateTime nextScannedDateTime, long nextScannedCustID);

        IEnumerable<PotentialPACustInfoEntity> GetCustInfos(string custIDs);
    }

    public class TWInitialGroupBettingDataAccess : ITWInitialGroupBettingDataAccess
    {
        private readonly IDynamicRepository repository;

        public TWInitialGroupBettingDataAccess()
        {
            repository = new DynamicRepository();
        }

        public TWInitialGBGetChangesModel GetChanges()
        {
            var criteria = new TWInitialGBGetChangesCriteria();

            return new TWInitialGBGetChangesModel
            {
                CustInfos = repository.Fetch<TWInitialGBGetChangesEntity>(criteria),
                NextScannedDateTime = criteria.NextScannedDateTime,
                NextScannedCustID = criteria.NextScannedCustID
            };
        }      

        public void UpdateNextScannedTime(DateTime nextScannedDateTime, long nextScannedCustID)
        {
            var criteria = new TWInitialGBUpdateNextScannedTimeCriteria
            {
                NextScannedDateTime = nextScannedDateTime,
                NextScannedCustID = nextScannedCustID
            };

            repository.Execute(criteria);
        }

        public IEnumerable<PotentialPACustInfoEntity> GetCustInfos(string custIDs)
        {
            var criteria = new GetCustInfoByCustIDCriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<PotentialPACustInfoEntity>(criteria);
        }
    }
}
