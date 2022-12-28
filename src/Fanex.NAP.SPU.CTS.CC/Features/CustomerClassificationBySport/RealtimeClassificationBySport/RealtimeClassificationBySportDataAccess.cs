namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Criteria;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;

    public interface IRealtimeClassificationBySportDataAccess
    {
        IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerBySportHasChangeForRunRealtime();

        IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerBySportForRunRealtime(string custSportJson);

        void ClassifyCustomerBySportRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customers);

        void UpdateLastScanRealtimeBySport(bool isGeneralFlow);
    }

    public class RealtimeClassificationBySportDataAccess : IRealtimeClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public RealtimeClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerBySportHasChangeForRunRealtime()
        {
            var criteria = new GetCustHasChangeForScanningCriteria();

            return repository.Fetch<CustClassificationRealtimeBySportEntity>(criteria);
        }

        public IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerBySportForRunRealtime(string custSportJson)
        {
            var criteria = new GetCustClassificationBySportByCustIDCriteria
            {
                CustSportsJson = custSportJson
            };

            return repository.Fetch<CustClassificationRealtimeBySportEntity>(criteria);
        }

        public void ClassifyCustomerBySportRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customers)
        {
            var criteria = new NormalAccountsClassificationBySportCategorizeRealtimeCriteria
            {
                CustomersXML = customers.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void UpdateLastScanRealtimeBySport(bool isGeneralFlow)
        {
            var criteria = new UpdateLastScanBySportCriteria 
            { 
                IsGeneralFlow = isGeneralFlow
            };

            repository.Execute(criteria);
        }
    }
}