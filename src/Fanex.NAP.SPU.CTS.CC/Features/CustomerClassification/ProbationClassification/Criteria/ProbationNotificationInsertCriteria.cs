namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProbationClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProbationNotificationInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TotalScanned")]
        public int TotalScanned { get; set; }

        [SpParam(SqlParamName = "ip_TotalFailed")]
        public int TotalFailed { get; set; }

        [SpParam(SqlParamName = "ip_TotalGeneralFailed")]
        public int TotalGeneralFailed { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Notification_InsertProbationNotification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}