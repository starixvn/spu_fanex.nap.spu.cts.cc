namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RescanPADangerousCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_PA_Dangerous_ReScan";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
