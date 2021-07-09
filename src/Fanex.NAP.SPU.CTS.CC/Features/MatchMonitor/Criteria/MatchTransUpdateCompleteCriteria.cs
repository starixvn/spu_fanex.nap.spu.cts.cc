namespace Fanex.NAP.SPU.CTS.Features.MatchMonitor.Criteria
{
    using Fanex.Data.Repository;

    public class MatchTransUpdateCompleteCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
                => "CTS_DC_MatchMonitor_Rule_Complete";

        public override bool IsValid()
            => true;
    }
}