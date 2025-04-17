namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class MissingCustomerCheckCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerListJson { get; set; }

        [SpParam(SqlParamName = "ip_LastCTSCustID")]
        public ulong? LastCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_QueryTypeID")]
        public int QueryTypeID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustomerLatency_CheckMissingCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}