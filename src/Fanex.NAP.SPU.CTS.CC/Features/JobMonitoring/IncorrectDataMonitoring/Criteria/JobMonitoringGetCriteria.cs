namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using Fanex.Data.Repository;

    public class JobMonitoringGetCriteria : CriteriaBase
    {
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