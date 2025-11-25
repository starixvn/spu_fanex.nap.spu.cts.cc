namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ClassifyCompletedMatchMonitorBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_MatchID")]
        public int MatchID { get; set; }

        [SpParam(SqlParamName = "ip_MMDetailsIDList")]
        public string MMDetailsJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_MatchMonitor_Completed";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
