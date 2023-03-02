namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustomerCategoryMonitorGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Monitoring_GetCategoryByCustIDs";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}