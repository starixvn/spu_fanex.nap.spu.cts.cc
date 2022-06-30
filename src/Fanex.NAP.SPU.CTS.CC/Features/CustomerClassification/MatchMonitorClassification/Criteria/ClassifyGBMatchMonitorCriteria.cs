namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ClassifyGBMatchMonitorCriteria : NonQueryCommand
    {       
        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_MatchMonitor_Classify";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
