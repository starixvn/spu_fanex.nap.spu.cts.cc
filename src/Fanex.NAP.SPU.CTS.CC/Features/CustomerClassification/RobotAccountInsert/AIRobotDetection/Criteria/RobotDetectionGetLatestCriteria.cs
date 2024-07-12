namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RobotDetectionGetLatestCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_RobotDetectionLastID")]
        public long LastRobotAIID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RobotDetection_GetLatest";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}