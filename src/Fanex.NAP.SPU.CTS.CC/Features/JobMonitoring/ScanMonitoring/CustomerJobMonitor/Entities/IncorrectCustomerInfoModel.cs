namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using System.Collections.Generic;

    public class IncorrectCustomerInfoModel
    {
        public ulong? NextScannedCTSCustId { get; set; }

        public IEnumerable<long> CustomerInfo { get; set; }
    }
}