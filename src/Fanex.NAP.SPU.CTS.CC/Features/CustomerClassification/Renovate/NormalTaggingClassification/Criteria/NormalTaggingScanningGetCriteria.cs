namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NormalTaggingScanningGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_LastCategoryID")]
        public int LastCategoryID { get; set; }

        [SpParam(SqlParamName = "op_LastCustID")]
        public ulong LastCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_TaggingScan_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
