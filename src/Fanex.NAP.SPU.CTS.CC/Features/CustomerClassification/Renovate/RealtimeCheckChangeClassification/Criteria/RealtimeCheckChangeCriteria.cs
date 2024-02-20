namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class RealtimeCheckChangeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "NextScannedDateTime1")]
        public DateTime? NextScannedDateTime1 { get; set; }

        [SpParam(SqlParamName = "NextScannedDateTime2")]
        public DateTime? NextScannedDateTime2 { get; set; }

        [SpParam(SqlParamName = "NextScannedDateTime3")]
        public DateTime? NextScannedDateTime3 { get; set; }

        [SpParam(SqlParamName = "NextScannedDateTime4")]
        public DateTime? NextScannedDateTime4 { get; set; }

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