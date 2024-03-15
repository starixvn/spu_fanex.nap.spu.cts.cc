namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousScoreClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckIsDangerousDetectionByCustCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public int CustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_CheckIsDangerousScore";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
