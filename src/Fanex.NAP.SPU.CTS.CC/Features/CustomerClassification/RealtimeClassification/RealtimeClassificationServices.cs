namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRealtimeClassificationServices
    {
        void CheckNewChangesForRealtime();

        IRealtimeClassificationServices Start();

        IRealtimeClassificationServices GetNewPlaceBetCustomers();

        IRealtimeClassificationServices GetCustomerCategory(int batchSize);

        IRealtimeClassificationServices ClassifyRealtimePerformance(int batchSize);

        IRealtimeClassificationServices UpdateGeneralRealtimeLastScan();

        IRealtimeClassificationServices UpdateLastTicketAssociationCustomer(int internalBatchSize);

        IRealtimeClassificationServices InsertPADailyQueue(int batchSize);
    }

    public class RealtimeClassificationServices : IRealtimeClassificationServices
    {
        private readonly IRealtimeClassificationDataAccess realtimeClassificationDataAccess;
        private IEnumerable<RealtimeNewPlaceBetCustomerEntity> customers;
        private List<CustomerCategoryRealtimeEntity> custCategoryRealtime;
        private List<long> custPADailyScan;
        private readonly ISentryClient sentryClient;

        public RealtimeClassificationServices(IRealtimeClassificationDataAccess realtimeClassificationDataAccess, ISentryClient sentryClient)
        {
            this.realtimeClassificationDataAccess = realtimeClassificationDataAccess;
            this.sentryClient = sentryClient;
        }

        public IRealtimeClassificationServices Start()
        {
            customers = new List<RealtimeNewPlaceBetCustomerEntity>();
            custCategoryRealtime = new List<CustomerCategoryRealtimeEntity>();
            custPADailyScan = new List<long>();
            return this;
        }

        public void CheckNewChangesForRealtime()
        {
            realtimeClassificationDataAccess.CheckNewChangesForRealtime();
        }

        public IRealtimeClassificationServices GetNewPlaceBetCustomers()
        {
            this.customers = realtimeClassificationDataAccess.GetNewPlaceBetCustomers();

            return this;
        }

        public IRealtimeClassificationServices GetCustomerCategory(int batchSize)
        {
            if (customers != null && customers.Any())
            {
                var customerChunks = customers.Distinct().Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    var custIDs = string.Join(",", custBatch.Select(c => c.CustId));

                    var customerCategory = realtimeClassificationDataAccess.GetCustomerCategory(custIDs);

                    custCategoryRealtime.AddRange(customerCategory.CustCategoryRealtime);

                    custPADailyScan.AddRange(customerCategory.CustPADailyScan);
                }
            }

            return this;
        }

        public IRealtimeClassificationServices ClassifyRealtimePerformance(int batchSize)
        {
            if (custCategoryRealtime.Any())
            {
                var custChunks = custCategoryRealtime.Chunk(batchSize);

                foreach (var custBatch in custChunks)
                {
                    var custCategoryForRealtime = custBatch.Where(c => c.CategoryId != -1);

                    realtimeClassificationDataAccess.ClassifyRealtimePerformance(custCategoryForRealtime);
                }
            }

            return this;
        }

        public IRealtimeClassificationServices UpdateGeneralRealtimeLastScan()
        {
            realtimeClassificationDataAccess.UpdateGeneralRealtimeLastScan(true);

            return this;
        }

        public IRealtimeClassificationServices UpdateLastTicketAssociationCustomer(int batchSize)
        {
            if (this.customers != null && this.customers.Any())
            {
                var custIds = this.customers.Select(x => new { CustID = x.CustId });

                var custIdChunks = custIds.Distinct().Chunk(batchSize);

                foreach (var custIdItems in custIdChunks)
                {
                    var custIdList = string.Join(",", custIdItems.Select(x => x.CustID));

                    realtimeClassificationDataAccess.UpdateAssociationCustomerLastTicket(custIdList);
                }
            }

            return this;
        }

        public IRealtimeClassificationServices InsertPADailyQueue(int batchSize)
        {
            if (custPADailyScan.Any())
            {
                var custChunks = custPADailyScan.Distinct().Chunk(batchSize);

                foreach (var custBatch in custChunks)
                {
                    realtimeClassificationDataAccess.InsertPADailyScanQueue(string.Join(",", custBatch));
                }
            }

            return this;
        }
    }
}