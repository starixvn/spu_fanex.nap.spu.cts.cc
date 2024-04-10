namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CompleteCustomerRealtimeBySportCriteria : NonQueryCommand
    {
        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_RealTime_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
