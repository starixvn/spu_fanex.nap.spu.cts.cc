namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NormalTaggingScannningGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int NumberOfRecord { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DailyScan_GetTagging";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
