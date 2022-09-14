namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustClassificationBySportByCustIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportsJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_GetByCustIDs";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}