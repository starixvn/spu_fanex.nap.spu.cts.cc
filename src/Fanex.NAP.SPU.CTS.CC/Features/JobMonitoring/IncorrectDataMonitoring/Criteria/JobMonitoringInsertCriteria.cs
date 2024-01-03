namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class JobMonitoringInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_IncorrectData")]
        public string IncorrectData { get; set; }

        [SpParam(SqlParamName = "ip_IssueTypeID")]
        public int IssueTypeID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Monitoring_IncorrectData_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}