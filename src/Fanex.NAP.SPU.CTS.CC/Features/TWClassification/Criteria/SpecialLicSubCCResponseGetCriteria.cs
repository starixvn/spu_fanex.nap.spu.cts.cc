namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class SpecialLicSubCCResponseGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialLicSubCC_Response_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
