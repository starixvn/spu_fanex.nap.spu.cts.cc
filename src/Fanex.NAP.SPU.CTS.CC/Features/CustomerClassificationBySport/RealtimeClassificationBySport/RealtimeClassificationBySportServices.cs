namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
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
                    var custCategory = dataAccess.GetCustomerCategory(JsonConvert.SerializeObject(custBatch));

                    customerCategories.CustomerNormals.AddRange(custCategory.CustomerNormals);
                    customerCategories.CustomerPADailyScans.AddRange(custCategory.CustomerPADailyScans);
                }
            }

            return this;
        }

        public IRealtimeClassificationBySportServices PreprocessCustomerClassRealtime(int batchSize)
        {                       
            if (customerCategories.CustomerNormals.IsNotNullOrEmpty())
            {
                var customerChunks = customerCategories.CustomerNormals.Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    dataAccess.PreprocessCustomerClassRealtime(custBatch);
                }
            }            

            return this;
        }          

        public IRealtimeClassificationBySportServices InsertPADailyScanQueue(int batchSize)
        {
            if (customerCategories.CustomerPADailyScans.IsNotNullOrEmpty())
            {
                var customerChunks = customerCategories.CustomerPADailyScans.Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    dataAccess.InsertPADailyScanQueue(JsonConvert.SerializeObject(custBatch));
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
