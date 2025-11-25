namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ClassifyMatchMonitorBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_MatchMonitor_Classify";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
