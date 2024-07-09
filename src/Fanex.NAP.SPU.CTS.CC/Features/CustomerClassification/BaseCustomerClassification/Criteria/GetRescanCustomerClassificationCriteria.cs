namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRescanCustomerClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_ReScan_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
