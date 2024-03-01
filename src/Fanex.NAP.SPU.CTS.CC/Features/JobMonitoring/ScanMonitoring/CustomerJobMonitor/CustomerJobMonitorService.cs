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

        IEnumerable<CustomerCategoryMonitorEntity> GetCustomerCategoryToMonitor(IEnumerable<long> custIds, int batchSize);

        IncorrectCustomerInfoModel CheckIncorrectCustomerInfo(ulong ctsCustID);
    }

    public class CustomerJobMonitorService : ICustomerJobMonitorService
    {
        private readonly ICustomerJobMonitorDataAccess customerJobMonitorDataAccess;
        private readonly IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess;

        public CustomerJobMonitorService(
                ICustomerJobMonitorDataAccess customerJobMonitorDataAccess,
                IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess)
        {
            this.customerJobMonitorDataAccess = customerJobMonitorDataAccess;
            this.classificationJobMonitorDataAccess = classificationJobMonitorDataAccess;
        }

        public MonitoringNewCustomerViewModel GetCustInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustID)
        {
            return (lastScannedCustID != 0) ? customerJobMonitorDataAccess.GetCustInfoFromMainDBForMonitor(batchSize, toDate, lastScannedCustID) : new MonitoringNewCustomerViewModel();
        }

        public MonitoringNewCustomerViewModel GetCustSubInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustSubID)
        {
            return (lastScannedCustSubID != 0) ? customerJobMonitorDataAccess.GetCustSubInfoFromMainDBForMonitor(batchSize, toDate, lastScannedCustSubID) : new MonitoringNewCustomerViewModel();
        }

        public IEnumerable<MissingCustomerCompareEntity> CheckMissingCustomer(IEnumerable<MonitoringNewCustomerEntity> customerList)
        {
            if (customerList != null && customerList.Any())
            {
                var customerJson = JsonConvert.SerializeObject(customerList);

                return customerJobMonitorDataAccess.CheckMissingCustomer(customerJson) ?? Enumerable.Empty<MissingCustomerCompareEntity>();
            }

            return Enumerable.Empty<MissingCustomerCompareEntity>();
        }

        public IncorrectCustomerInfoModel CheckIncorrectCustomerInfo(ulong ctsCustID)
        {
            if (ctsCustID > 0)
            {
                return customerJobMonitorDataAccess.CheckIncorrectCustomerInfo(ctsCustID);
            }

            return new IncorrectCustomerInfoModel();
        }

        public IEnumerable<CustomerCategoryMonitorEntity> GetCustomerCategoryToMonitor(IEnumerable<long> custIds, int batchSize)
        {
            var custCategory = new List<CustomerCategoryMonitorEntity>();

            if (custIds.Any())
            {
                var custChunks = custIds.Distinct().Chunk(batchSize);

                foreach (var custBatch in custChunks)
                {
                    var custToString = string.Join(',', custBatch);

                    custCategory.AddRange(classificationJobMonitorDataAccess.GetCustomerCategoryToMonitor(custToString, true));
                }
            }

            return custCategory;
        }
    }
}