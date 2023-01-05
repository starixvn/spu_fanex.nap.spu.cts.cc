namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Criteria
{
    using Fanex.Data.Repository;

    public class RealtimeNewChangesCheckCriteria : NonQueryCommand
    {
        public int MinuteTimeRange { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_RealTime_CheckChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}