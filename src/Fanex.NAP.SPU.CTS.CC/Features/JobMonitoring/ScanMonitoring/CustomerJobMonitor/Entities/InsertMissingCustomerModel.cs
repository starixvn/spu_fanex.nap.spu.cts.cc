namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class InsertMissingCustomerModel
    {
        public IEnumerable<MissingCustomerCompareEntity> MissingCustomers { get; set; }

        public JobMonitor_IssueType MissingCustomerIssueType { get; set; }

        public IEnumerable<MonitoringNewCustomerEntity> CustomerInfo { get; set; }
    }
}