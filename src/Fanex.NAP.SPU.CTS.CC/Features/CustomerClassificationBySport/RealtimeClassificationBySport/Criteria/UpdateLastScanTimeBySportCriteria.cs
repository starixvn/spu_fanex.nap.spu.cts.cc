namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;

    public class UpdateLastScanTimeBySportCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_BySport_Realtime_UpdateLastScanTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}