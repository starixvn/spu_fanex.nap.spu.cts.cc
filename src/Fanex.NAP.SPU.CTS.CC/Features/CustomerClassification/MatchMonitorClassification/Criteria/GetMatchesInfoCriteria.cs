namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMatchesInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "MatchIds")]
        public string MatchIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_MatchInfo_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
