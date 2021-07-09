namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;

    public class RealtimeLastScanTimeUpdateCriteria : NonQueryCommand
    {
        public bool IsGeneralCategory { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_UpdateLastScanTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}