namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalTaggingClassification.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class NormalTaggingScanningCheckChangesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_TaggingCust")]
        public string TaggingCust { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_TaggingScan_CheckChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}