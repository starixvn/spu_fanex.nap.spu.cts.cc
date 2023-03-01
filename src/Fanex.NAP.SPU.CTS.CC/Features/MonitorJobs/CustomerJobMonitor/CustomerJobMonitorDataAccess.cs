namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using System;
    using System.Collections.Generic;

    public interface ICustomerJobMonitorDataAccess
    {
        IEnumerable<CustomerMainDBModel> GetCustInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<CustomerMainDBModel> GetCustSubInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<MissingCustomerCompareEntity> CompareCustomerDataForMonitor(string custDataJson);

        void InsertMonitorMissingCust(string custDataJson, DateTime scannedDate);
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

        public IEnumerable<MissingCustomerCompareEntity> CompareCustomerDataForMonitor(string custDataJson)
        {
            var criteria = new CompareCustomerDataForMonitorCriteria
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
    }
}