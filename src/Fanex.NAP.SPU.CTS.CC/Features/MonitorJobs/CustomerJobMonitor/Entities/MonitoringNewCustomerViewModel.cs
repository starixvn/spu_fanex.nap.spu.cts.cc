namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using System.Collections.Generic;

    public class MonitoringNewCustomerViewModel
    {
        public IEnumerable<MonitoringNewCustomerEntity> NewCustomers { get; set; }

        public long? MaxScannedCustomerID { get; set; }
    }
}