namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CompleteCustomerRealtimeCriteria : NonQueryCommand
    {    
        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_RealTime_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
