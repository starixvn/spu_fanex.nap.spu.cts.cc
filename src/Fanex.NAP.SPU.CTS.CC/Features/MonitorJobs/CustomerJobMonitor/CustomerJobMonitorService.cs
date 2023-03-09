namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerJobMonitorService
    {
        MonitoringNewCustomerViewModel GetCustInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustID);

        MonitoringNewCustomerViewModel GetCustSubInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustSubID);

        IEnumerable<MissingCustomerCompareEntity> CheckMissingCustomer(IEnumerable<MonitoringNewCustomerEntity> customerList);

        void InsertMonitorMissingCust(IEnumerable<MissingCustomerCompareEntity> customerList, DateTime scannedDate, int batchSize);

        IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(IEnumerable<long> custIds, int batchSize);

        void InsertMonitorInvalidCustomerCategory(IEnumerable<IncorrectCustomerCategoriesMonitorEntity> customerList, DateTime scannedDate, JobMonitorType type, int batchSize);
    }

    public class CustomerJobMonitorService : ICustomerJobMonitorService
    {
        private readonly ICustomerJobMonitorDataAccess customerJobMonitorDataAccess;

        public CustomerJobMonitorService(
                ICustomerJobMonitorDataAccess customerJobMonitorDataAccess)
        {
            this.customerJobMonitorDataAccess = customerJobMonitorDataAccess;
        }

        public MonitoringNewCustomerViewModel GetCustInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustID)
        {
            return (lastScannedCustID != 0) ? customerJobMonitorDataAccess.GetCustInfoFromMainDBForMonitor(batchSize, toDate, lastScannedCustID) : new MonitoringNewCustomerViewModel();
        }

        public MonitoringNewCustomerViewModel GetCustSubInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustSubID)
        {
            return (lastScannedCustSubID != 0) ? customerJobMonitorDataAccess.GetCustSubInfoFromMainDBForMonitor(batchSize, toDate, lastScannedCustSubID) : new MonitoringNewCustomerViewModel();
        }

        public void InsertMonitorMissingCust(IEnumerable<MissingCustomerCompareEntity> customerList, DateTime scannedDate, int batchSize)
        {
            if (customerList.Any())
            {
                var customerChunks = customerList.ToList().Chunk(batchSize);

                foreach (var custBatch in customerChunks)
                {
                    var customerJson = JsonConvert.SerializeObject(custBatch);

                    customerJobMonitorDataAccess.InsertMonitorMissingCust(customerJson, scannedDate);
                }
            }
        }

        public IEnumerable<MissingCustomerCompareEntity> CheckMissingCustomer(IEnumerable<MonitoringNewCustomerEntity> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return customerJobMonitorDataAccess.CheckMissingCustomer(customerJson) ?? Enumerable.Empty<MissingCustomerCompareEntity>();
        }

        public IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(IEnumerable<long> custIds, int batchSize)
        {
            var custCategory = new List<CustomerCategoriesMonitorEntity>();

            if (custIds.Any())
            {
                var custChunks = custIds.Distinct().Chunk(batchSize);

                foreach (var custBatch in custChunks)
                {
                    var custToString = string.Join(',', custBatch);

                    custCategory.AddRange(customerJobMonitorDataAccess.GetCustomerCategoriesToMonitor(custToString));
                }
            }

            return custCategory;
        }

        public void InsertMonitorInvalidCustomerCategory(IEnumerable<IncorrectCustomerCategoriesMonitorEntity> customerList, DateTime scannedDate, JobMonitorType type, int batchSize)
        {
            if (customerList.Any())
            {
                var custChunks = customerList.Distinct().Chunk(batchSize);

                foreach (var custBatch in custChunks)
                {
                    var customerJson = JsonConvert.SerializeObject(custBatch);

                    customerJobMonitorDataAccess.InsertMonitorIncorrectCustCategory(customerJson, scannedDate, type);
                }
            }
        }
    }
}