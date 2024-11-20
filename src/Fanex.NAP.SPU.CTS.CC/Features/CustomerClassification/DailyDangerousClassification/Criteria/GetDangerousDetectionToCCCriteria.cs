namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDangerousDetectionToCCCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsLicensee")]
        public bool IsLicensee { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_DangerousDetection_GetLatest";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}