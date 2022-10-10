namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCTSCustomerClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListID")]
        public string IDList { get; set; }

        [SpParam(SqlParamName = "ip_IsCTSCustID")]
        public bool IsCTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
