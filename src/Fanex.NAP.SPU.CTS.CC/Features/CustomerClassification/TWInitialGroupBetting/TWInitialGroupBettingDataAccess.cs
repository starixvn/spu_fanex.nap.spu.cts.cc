namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWInitialGroupBetting
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ITWInitialGroupBettingDataAccess
    {
        TWInitialGBGetChangesModel GetChanges();

        IEnumerable<CustInfoEntity> GetCustInfos(IEnumerable<long> custIDs);

        void UpdateNextScannedTime(DateTime nextScannedDateTime);
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
                CustIDs = repository.Fetch<TWInitialGBGetChangesEntity>(criteria),
                NextScannedDateTime = criteria.NextScannedDateTime
            };
        }

        public IEnumerable<CustInfoEntity> GetCustInfos(IEnumerable<long> custIDs)
        {
            var criteria = new GetCustInfoByCustIDCriteria
            {
                 CustIDs = JsonConvert.SerializeObject(custIDs)
            };

            return repository.Fetch<CustInfoEntity>(criteria);
        }

        public void UpdateNextScannedTime(DateTime nextScannedDateTime)
        {
            var criteria = new TWInitialGBUpdateNextScannedTimeCriteria
            {
                NextScannedDateTime = nextScannedDateTime
            };

            repository.Execute(criteria);
        }
    }
}
