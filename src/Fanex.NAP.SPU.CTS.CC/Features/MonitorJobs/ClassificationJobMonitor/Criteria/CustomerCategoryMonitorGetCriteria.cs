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
            return "CTS_DC_CustClassification_Monitor_GetCategoryIDs";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}