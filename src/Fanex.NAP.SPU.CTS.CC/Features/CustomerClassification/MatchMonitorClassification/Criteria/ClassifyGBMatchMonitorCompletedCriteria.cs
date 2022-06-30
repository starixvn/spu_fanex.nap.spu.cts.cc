namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ClassifyGBMatchMonitorCompletedCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_MatchID")]
        public int MatchID { get; set; }

        [SpParam(SqlParamName = "ip_MMDetailsList")]
        public string MMDetailsJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_MatchMonitor_Completed";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
