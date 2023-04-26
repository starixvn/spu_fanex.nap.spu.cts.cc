namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification
{
    using Fanex.Data.Repository;
    using System;

    public class RealtimeCheckChangeUpdateNextScanCriteria : NonQueryCommand
    {
        public DateTime NextScannedDateTime { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalClassification_RealTime_UpdateNextScannedTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
