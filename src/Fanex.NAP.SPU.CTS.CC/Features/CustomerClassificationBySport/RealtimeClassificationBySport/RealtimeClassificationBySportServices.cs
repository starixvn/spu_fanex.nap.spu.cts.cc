namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IRealtimeClassificationBySportServices
    {
        IRealtimeClassificationBySportServices Start();

        IRealtimeClassificationBySportServices GetNewPlaceBetCustomers();

        IRealtimeClassificationBySportServices GetCustomerCategory(int batchSize);

        IRealtimeClassificationBySportServices PreprocessCustomerClassRealtime(int batchSize);

        IRealtimeClassificationBySportServices InsertPADailyScanQueue(int batchSize);

        IRealtimeClassificationBySportServices CompleteCustomerRealtime();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Datetime Code Smell", "S6566:Prefer using DateTimeOffset instead of DateTime", Justification = "<NotHandle>")]
    public class RealtimeClassificationBySportServices : IRealtimeClassificationBySportServices
    {
        private readonly IRealtimeClassificationBySportDataAccess dataAccess;
        private GetNewPlaceBetCustomerModel customers;
        private GetCustomerCategoryBySportModel customerCategories;


        // private const string ServiceName = "Realtime Classification By Sport Service";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
        public RealtimeClassificationBySportServices(IRealtimeClassificationBySportDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;            
        }

        public IRealtimeClassificationBySportServices Start()
        {
            customers = new GetNewPlaceBetCustomerModel();
            customerCategories = new GetCustomerCategoryBySportModel();

            return this;
        }

        public IRealtimeClassificationBySportServices GetNewPlaceBetCustomers()
        {
            customers = dataAccess.GetNewPlaceBetCustomers();

            return this;
        }       

        public IRealtimeClassificationBySportServices GetCustomerCategory(int batchSize)
        {
            if (customers != null && customers.CustomerNewPlaceBets != null && customers.CustomerNewPlaceBets.Any())
            {
                var customerChunks = customers.CustomerNewPlaceBets.Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    var custIDs = string.Join(",", custBatch.Select(c => c.CustId).Distinct());

                    var custCategory = dataAccess.GetCustomerCategory(custIDs);

                    customerCategories.CustomerNormals.AddRange(custCategory.CustomerNormals);
                    customerCategories.CustomerPADailyScans.AddRange(custCategory.CustomerPADailyScans);
                }
            }

            return this;
        }

        public IRealtimeClassificationBySportServices PreprocessCustomerClassRealtime(int batchSize)
        {
            if (!HasValidInput())
            {
                return this;
            }

            var joinedCustomers = GetJoinedCustomers();

            if (!joinedCustomers.Any())
            {
                return this;
            }

            var customerChunks = joinedCustomers.Chunk(batchSize);

            foreach (var custBatch in customerChunks)
            {
                dataAccess.PreprocessCustomerClassRealtime(custBatch);
            }

            return this;
        }

        private bool HasValidInput()
        {
            var hasCustomers = customers?.CustomerNewPlaceBets?.Any() ?? false;
            var hasCustomerNormals = customerCategories?.CustomerNormals?.Any() ?? false;

            return hasCustomers && hasCustomerNormals;
        }

        private List<CustClassificationRealtimeBySportEntity> GetJoinedCustomers()
        {
            return customers.CustomerNewPlaceBets
                .Where(c => customerCategories.CustomerNormals.Contains(c.CustId))
                .ToList();
        }       

        public IRealtimeClassificationBySportServices InsertPADailyScanQueue(int batchSize)
        {
            if (customerCategories.CustomerPADailyScans.Any())
            {
                var customerChunks = customerCategories.CustomerPADailyScans.Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    dataAccess.InsertPADailyScanQueue(string.Join(",", custBatch));
                }
            }

            return this;
        }

        public IRealtimeClassificationBySportServices CompleteCustomerRealtime()
        {
            if (customers != null && customers.ScannedMaxId > 0)
            {
                dataAccess.CompleteCustomerRealtime(customers.ScannedMaxId);
            }

            return this;
        }
    }
}
