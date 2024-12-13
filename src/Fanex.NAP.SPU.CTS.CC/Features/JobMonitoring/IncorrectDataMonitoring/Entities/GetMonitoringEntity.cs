namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using System;

    public class GetMonitoringEntity
    {
        public Int64 ID { get; set; }

        public int IssueTypeID { get; set; }

        public string TrackingInfo { get; set; }

        public int? CustID { get; set; }

        public long? Value { get; set; }

        public bool IsResolved { get; set; }

        public bool IsIssueTypeValid { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}