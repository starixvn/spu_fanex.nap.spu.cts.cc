namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class RealtimeCheckChangeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "NextScannedDateTime")]
        public DateTime? NextScannedDateTime { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalClassification_RealTime_CheckChanges";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
