namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustInfoByCustIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey() => "CTS_DC_CustClassification_InitialGB_GetCustInfo";

        public override bool IsValid() => true;
    }
}
