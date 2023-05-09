namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRealtimeClassificationService
    {
        IRealtimeClassificationService Start();

        IRealtimeClassificationService GetNewPlaceBetCustomers();

        IRealtimeClassificationService GetCustomerCategory(int batchSize);

        IRealtimeClassificationService PreprocessCustomerClassRealtime(int batchSize);

        IRealtimeClassificationService InsertPADailyScanQueue(int batchSize);

        IRealtimeClassificationService CompleteCustomerRealtime();

    }

    public class RealtimeClassificationService : IRealtimeClassificationService
    {
        private readonly IRealtimeClassificationDataAccess dataAccess;
        private readonly ISentryClient sentryClient;
        private GetNewPlaceBetCustomerModel customers;
        private GetCustomerCategoryRealtimeModel customerCategories;

        public RealtimeClassificationService(IRealtimeClassificationDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public IRealtimeClassificationService Start()
        {
            this.customers = new GetNewPlaceBetCustomerModel();
            this.customerCategories = new GetCustomerCategoryRealtimeModel();

            return this;
        }

        public IRealtimeClassificationService GetNewPlaceBetCustomers()
        {
            this.customers = this.dataAccess.GetNewPlaceBetCustomers();
           
            return this;
        }

        public IRealtimeClassificationService GetCustomerCategory(int batchSize)
        {
            if (this.customers != null && this.customers.CustomerNewPlaceBets != null && this.customers.CustomerNewPlaceBets.Any())
            {
                var customerChunks = customers.CustomerNewPlaceBets.Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    var custIDs = string.Join(",", custBatch.Select(c => c.CustId).Distinct());

                    var custCategory = dataAccess.GetCustomerCategory(custIDs);

                    this.customerCategories.CustomerCategory.AddRange(custCategory.CustomerCategory);
                    this.customerCategories.CustomerPADailyScan.AddRange(custCategory.CustomerPADailyScan);                 
                }
            } 

            return this;
        }

        public IRealtimeClassificationService PreprocessCustomerClassRealtime(int batchSize)
        {
            if (this.customerCategories.CustomerCategory.Any())
            {
                var customerChunks = this.customerCategories.CustomerCategory.Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    var custIdList = string.Join(",", custBatch.Select(x =>x.CustId));                   

                    dataAccess.PreprocessCustomerClassRealtime(custIdList);
                }
            }

            return this;
        }
        
        public IRealtimeClassificationService InsertPADailyScanQueue(int batchSize)
        {
            if (this.customerCategories.CustomerPADailyScan.Any())
            {
                var customerChunks = this.customerCategories.CustomerPADailyScan.Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    dataAccess.InsertPADailyScanQueue(string.Join(",", custBatch));
                }
            }

            return this;
        }

        public IRealtimeClassificationService CompleteCustomerRealtime()
        {
            if (this.customers != null && this.customers.ScannedMaxId > 0)
            {
                this.dataAccess.CompleteCustomerRealtime(this.customers.ScannedMaxId);
            }

            return this;
        }

    }
}
