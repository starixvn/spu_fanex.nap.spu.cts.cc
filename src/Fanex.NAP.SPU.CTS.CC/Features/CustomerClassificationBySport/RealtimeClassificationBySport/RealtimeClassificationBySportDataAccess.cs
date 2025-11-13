namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{    
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRealtimeClassificationBySportDataAccess
    {
        GetNewPlaceBetCustomerModel GetNewPlaceBetCustomers();

        GetCustomerCategoryBySportModel GetCustomerCategory(string custIdList);

        void PreprocessCustomerClassRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customers);

        void InsertPADailyScanQueue(string custIDList);      

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

        public GetCustomerCategoryBySportModel GetCustomerCategory(string custIdList)
        {
            var criteria = new GetCustomerCategoryRealtimeBySportCriteria
            {
                CustIdList = custIdList
            };

            var (normalCusts, paCusts) = repository.FetchMultiple<long, long>(criteria);
            
            return new GetCustomerCategoryBySportModel
            {
                  CustomerNormals = normalCusts?.ToList(),
                  CustomerPADailyScans = paCusts?.ToList()
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

        public void InsertPADailyScanQueue(string custIDList)
        {
            var criteria = new InsertQueuePADailyBySportCriteria
            {
                CustIDList = custIDList
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
