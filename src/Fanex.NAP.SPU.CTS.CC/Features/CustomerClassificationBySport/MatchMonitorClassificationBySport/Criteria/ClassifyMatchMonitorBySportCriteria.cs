namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    using Fanex.Data.Repository;

    public class ClassifyMatchMonitorBySportCriteria : NonQueryCommand
    {
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
