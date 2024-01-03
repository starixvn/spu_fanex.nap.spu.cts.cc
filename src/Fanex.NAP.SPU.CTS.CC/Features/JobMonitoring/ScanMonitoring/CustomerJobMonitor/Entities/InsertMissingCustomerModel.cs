namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using System.Collections.Generic;

    public class InsertMissingCustomerModel
    {
        public IEnumerable<MissingCustomerCompareEntity> MissingCustomers { get; set; }

        public IEnumerable<MonitoringNewCustomerEntity> CustomerInfo { get; set; }
    }
}