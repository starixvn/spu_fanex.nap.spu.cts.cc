namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckTWTaggingExcludeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string JsonData { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_TWTagging_Exclude";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
} 