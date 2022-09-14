namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Criteria;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;

    public interface IRealtimeClassificationBySportDataAccess
    {
        IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerGeneralCategoryHasChangeForRunRealtime();

        IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerGeneralCategoryForRunRealtime(string custSportJson);

        void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customers);

        void RealtimeUpdateLastScanTime(bool isGeneralCategory);
    }

    public class RealtimeClassificationBySportDataAccess : IRealtimeClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public RealtimeClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerGeneralCategoryHasChangeForRunRealtime()
        {
            var criteria = new GetCustHasChangeForScanningCriteria();

            return repository.Fetch<CustClassificationRealtimeBySportEntity>(criteria);
        }

        public IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerGeneralCategoryForRunRealtime(string custSportJson)
        {
            var criteria = new GetCustClassificationBySportByCustIDCriteria
            {
                CustSportsJson = custSportJson
            };

            return repository.Fetch<CustClassificationRealtimeBySportEntity>(criteria);
        }

        public void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customers)
        {
            var criteria = new NormalAccountsClassificationBySportCategorizeRealtimeCriteria
            {
                CustomersXML = customers.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void RealtimeUpdateLastScanTime(bool isGeneralCategory)
        {
            var criteria = new UpdateLastScanTimeBySportCriteria();
            repository.Execute(criteria);
        }
    }
}