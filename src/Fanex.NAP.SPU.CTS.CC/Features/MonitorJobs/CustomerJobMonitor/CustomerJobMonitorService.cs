namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICustomerJobMonitorService
    {
        IEnumerable<CustomerMainDBModel> GetCustInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<CustomerMainDBModel> GetCustSubInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<MissingCustomerCompareEntity> CompareCustomerDataForMonitor(IEnumerable<CustomerMainDBModel> customerList);

        void InsertMonitorMissingCust(IEnumerable<MissingCustomerCompareEntity> customerList, DateTime scannedDate);
    }

    public class CustomerJobMonitorService : ICustomerJobMonitorService
    {
        private readonly ICustomerJobMonitorDataAccess customerJobMonitorDataAccess;

        public CustomerJobMonitorService(
                ICustomerJobMonitorDataAccess customerJobMonitorDataAccess)
        {
            this.customerJobMonitorDataAccess = customerJobMonitorDataAccess;            
        }

        public IEnumerable<CustomerMainDBModel> GetCustInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate)
        {
            return customerJobMonitorDataAccess.GetCustInfoFromMainDBForMonitor(fromDate, toDate) ?? Enumerable.Empty<CustomerMainDBModel>();
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate)
        {
            return customerJobMonitorDataAccess.GetCustSubInfoFromMainDBForMonitor(fromDate, toDate) ?? Enumerable.Empty<CustomerMainDBModel>();
        }

        public void InsertMonitorMissingCust(IEnumerable<MissingCustomerCompareEntity> customerList, DateTime scannedDate)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            customerJobMonitorDataAccess.InsertMonitorMissingCust(customerJson, scannedDate);
        }

        public IEnumerable<MissingCustomerCompareEntity> CompareCustomerDataForMonitor(IEnumerable<CustomerMainDBModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return customerJobMonitorDataAccess.CompareCustomerDataForMonitor(customerJson) ?? Enumerable.Empty<MissingCustomerCompareEntity>();
        }
    }
}