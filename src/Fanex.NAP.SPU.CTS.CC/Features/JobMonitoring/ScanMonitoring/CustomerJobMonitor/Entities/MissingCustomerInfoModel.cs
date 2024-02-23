namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using System.Collections.Generic;

    public class MissingCustomerInfoModel
    {
       public long? NextScannedCTSCustId { get; set; }

        public IEnumerable<long> CustomerInfo { get; set; }
    }
}