namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class RobotDetectionGetLatestCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_LatestTimeAI")]
        public DateTime LatestTimeAI { get; set; }

        [SpParam(SqlParamName = "ip_LatestTimeImperva")]
        public DateTime LatestTimeImperva { get; set; }

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