namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification.Model;
    using System;

    public interface ITWGBTaggingClassificationDataAccess
    {
        TWGBTaggingCheckChangeModel GetChanges();

        void UpdateNextScannedTime(DateTime nextScannedDateTime);

        void Preprocess(string listCustId);
    }

    public class TWGBTaggingClassificationDataAccess : ITWGBTaggingClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public TWGBTaggingClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public TWGBTaggingCheckChangeModel GetChanges()
        {
            var criteria = new TWGBTaggingCheckChangeCriteria();

            return new TWGBTaggingCheckChangeModel
            {
                CustIDs = repository.Fetch<long>(criteria),
                NextScannedDateTime = criteria.NextScannedDateTime
            };
        }

        public void UpdateNextScannedTime(DateTime nextScannedDateTime)
        {
            var criteria = new TWGBTaggingUpdateNextScannedTimeCriteria()
            {
                NextScannedDateTime = nextScannedDateTime
            };

            repository.Execute(criteria);
        }

        public void Preprocess(string listCustId)
        {
            var criteria = new TWGBTaggingPreprocessCriteria()
            {
                ListCustId = listCustId
            };

            repository.Execute(criteria);
        }
    }
}
