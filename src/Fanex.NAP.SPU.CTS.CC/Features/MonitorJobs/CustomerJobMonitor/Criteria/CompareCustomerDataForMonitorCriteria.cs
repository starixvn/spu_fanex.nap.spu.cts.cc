namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CompareCustomerDataForMonitorCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerListJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_Monitoring_CompareData";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerListJson);
        }
    }
}