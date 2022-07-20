namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{   
    using Fanex.Data.Repository;

    public class GetMatchMonitorCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_MatchMonitor_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
