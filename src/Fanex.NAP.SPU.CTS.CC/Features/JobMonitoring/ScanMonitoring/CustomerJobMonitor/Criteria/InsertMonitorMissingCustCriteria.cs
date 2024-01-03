namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class InsertMonitorMissingCustCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerListJson { get; set; }

        [SpParam(SqlParamName = "ip_ScannedDate")]
        public DateTime ScannedDate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Monitoring_InsertMissingCustomer";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerListJson);
        }
    }
}