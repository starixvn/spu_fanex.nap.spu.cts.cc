namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICustomerJobMonitorDataAccess
    {
        MonitoringNewCustomerViewModel GetCustInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustID);

        MonitoringNewCustomerViewModel GetCustSubInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustSubID);

        IEnumerable<MissingCustomerCompareEntity> CheckMissingCustomer(string custDataJson);
    }

    public class CustomerJobMonitorDataAccess : ICustomerJobMonitorDataAccess
    {
        private readonly IDynamicRepository repository;

        public CustomerJobMonitorDataAccess()
        {
            repository = new DynamicRepository();
        }

        public MonitoringNewCustomerViewModel GetCustInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustID)
        {
            var criteria = new GetCustomerInfoFromMainDBForMonitorCriteria
            {
                RowSize = batchSize,
                MaxScannedDate = toDate,
                LastCustId = lastScannedCustID
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var result = db.QueryMultiple(criteria);
            var maxScannedCustID = result?.Read<long?>();
            var newCustomers = result?.Read<MonitoringNewCustomerEntity>();

            return new MonitoringNewCustomerViewModel()
            {
                NewCustomers = newCustomers ?? Enumerable.Empty<MonitoringNewCustomerEntity>(),
                MaxScannedCustomerID = maxScannedCustID?.FirstOrDefault()
            };
        }

        public MonitoringNewCustomerViewModel GetCustSubInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustID)
        {
            var criteria = new GetCustSubInfoFromMainDBForMonitorCriteria
            {
                RowSize = batchSize,
                MaxScannedDate = toDate,
                LastCustSubId = lastScannedCustID
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var result = db.QueryMultiple(criteria);
            var maxScannedCustID = result?.Read<long?>();
            var newCustomers = result?.Read<MonitoringNewCustomerEntity>();

            return new MonitoringNewCustomerViewModel()
            {
                NewCustomers = newCustomers ?? Enumerable.Empty<MonitoringNewCustomerEntity>(),
                MaxScannedCustomerID = maxScannedCustID?.FirstOrDefault()
            };
        }

        public IEnumerable<MissingCustomerCompareEntity> CheckMissingCustomer(string custDataJson)
        {
            var criteria = new MissingCustomerCheckCriteria
            {
                CustomerListJson = custDataJson
            };

            return repository.Fetch<MissingCustomerCompareEntity>(criteria);
        }
    }
}