namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;

    public class LastScanRealtimeUpdateCriteria : NonQueryCommand
    {
        public bool IsGeneralFlow { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_Realtime_UpdateLastScan";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}