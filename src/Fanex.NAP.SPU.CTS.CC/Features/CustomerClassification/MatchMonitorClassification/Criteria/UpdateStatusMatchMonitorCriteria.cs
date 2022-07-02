namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateStatusMatchMonitorCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_MatchJson")]
        public string MatchStatusInfos { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_MatchMonitor_UpdateMatch";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
