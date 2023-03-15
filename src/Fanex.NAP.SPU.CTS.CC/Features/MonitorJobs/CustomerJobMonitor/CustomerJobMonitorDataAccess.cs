namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerJobMonitorDataAccess
    {
        MonitoringNewCustomerViewModel GetCustInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustID);

        MonitoringNewCustomerViewModel GetCustSubInfoFromMainDBForMonitor(int batchSize, DateTime toDate, long lastScannedCustSubID);

        IEnumerable<MissingCustomerCompareEntity> CheckMissingCustomer(string custDataJson);

        void InsertMonitorMissingCust(string custDataJson, DateTime scannedDate);

        IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(string custIds, bool IsCheckInternalAccount);

        void InsertMonitorIncorrectCustCategory(string custDataJson, DateTime scannedDate, JobMonitorType type);
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

        public void InsertMonitorMissingCust(string custDataJson, DateTime scannedDate)
        {
            var criteria = new InsertMonitorMissingCustCriteria
            {
                CustomerListJson = custDataJson,
                ScannedDate = scannedDate
            };

            repository.Execute(criteria);
        }

        public IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(string custIds, bool IsCheckInternalAccount)
        {
            var criteria = new CustomerCategoryMonitorGetCriteria()
            {
                CustIDs = custIds,
                IsCheckInternalAccount = IsCheckInternalAccount
            };

            return repository.Fetch<CustomerCategoriesMonitorEntity>(criteria);
        }

        public void InsertMonitorIncorrectCustCategory(string custDataJson, DateTime scannedDate, JobMonitorType type)
        {
            var criteria = new MonitorIncorrectCustCategoryInsertCriteria
            {
                CustomerListJson = custDataJson,
                InvalidType = (int)type,
                ScannedTime = scannedDate
            };

            repository.Execute(criteria);
        }
    }
}