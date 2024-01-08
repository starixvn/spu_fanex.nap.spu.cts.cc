namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class MonitoringNewCustomerViewModel
    {
        public MonitoringNewCustomerViewModel()
        {
            NewCustomers = Enumerable.Empty<MonitoringNewCustomerEntity>();
        }

        public IEnumerable<MonitoringNewCustomerEntity> NewCustomers { get; set; }

        public long? MaxScannedCustomerID { get; set; }
    }
}