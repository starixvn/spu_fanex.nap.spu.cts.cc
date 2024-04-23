namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyTWInitialGroupBetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustInfoByCustIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey() => "CTS_DC_CustClassification_InitialGB_GetCustInfo";

        public override bool IsValid() => true;
    }
}
