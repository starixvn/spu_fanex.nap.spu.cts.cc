namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NotificationMonitoringGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_IsContinue")]
        public bool IsContinue { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Monitoring_IncorrectData_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}