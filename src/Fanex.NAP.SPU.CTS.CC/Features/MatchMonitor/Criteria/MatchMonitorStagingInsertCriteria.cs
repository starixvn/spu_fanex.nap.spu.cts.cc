namespace Fanex.NAP.SPU.CTS.Features.MatchMonitor.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class MatchMonitorStagingInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransList")]
        public string MatchTransJson { get; set; }

        public override string GetSettingKey()
                => "CTS_DC_MatchMonitor_Staging_Insert";

        public override bool IsValid()
            => true;
    }
}