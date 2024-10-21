namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{    
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;

    public interface IRealtimeClassificationBySportDataAccess
    {
        GetNewPlaceBetCustomerModel GetNewPlaceBetCustomers();
        
        void PreprocessCustomerClassRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customers);

        void CompleteCustomerRealtime(long scannedMaxId);
    }

    public class RealtimeClassificationBySportDataAccess : IRealtimeClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public RealtimeClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNewPlaceBetCustomerModel GetNewPlaceBetCustomers()
        {
            var criteria = new GetChangeCustomerRealtimeBySportCriteria();

            var data = repository.Fetch<CustClassificationRealtimeBySportEntity>(criteria);

            return new GetNewPlaceBetCustomerModel
            {
                CustomerNewPlaceBets = data,
                ScannedMaxId = criteria.ScannedMaxId
            };
        }       

        public void PreprocessCustomerClassRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customers)
        {
            var criteria = new PreprocessCustomerRealtimeBySportCriteria
            {
                CustomersXML = customers.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void CompleteCustomerRealtime(long scannedMaxId)
        {
            var criteria = new CompleteCustomerRealtimeBySportCriteria
            {
                ScannedMaxId = scannedMaxId
            };

            repository.Execute(criteria);
        }

    }
}
