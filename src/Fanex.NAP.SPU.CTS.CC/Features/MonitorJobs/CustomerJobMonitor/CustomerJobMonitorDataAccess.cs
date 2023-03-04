namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerJobMonitorDataAccess
    {
        IEnumerable<CustomerMainDBModel> GetCustInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<CustomerMainDBModel> GetCustSubInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<MissingCustomerCompareEntity> CheckMissingCustomer(string custDataJson);

        void InsertMonitorMissingCust(string custDataJson, DateTime scannedDate);

        IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(string custIds);

        void InsertMonitorIncorrectCustCategory(string custDataJson, DateTime scannedDate, JobMonitorType type);
    }

    public class CustomerJobMonitorDataAccess : ICustomerJobMonitorDataAccess
    {
        private readonly IDynamicRepository repository;

        public CustomerJobMonitorDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustomerMainDBModel> GetCustInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetCustomerInfoFromMainDBForMonitorCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return repository.Fetch<CustomerMainDBModel>(criteria);
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetCustSubInfoFromMainDBForMonitorCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            return repository.Fetch<CustomerMainDBModel>(criteria);
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

        public IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(string custIds)
        {
            var criteria = new CustomerCategoryMonitorGetCriteria()
            {
                CustIDs = custIds
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