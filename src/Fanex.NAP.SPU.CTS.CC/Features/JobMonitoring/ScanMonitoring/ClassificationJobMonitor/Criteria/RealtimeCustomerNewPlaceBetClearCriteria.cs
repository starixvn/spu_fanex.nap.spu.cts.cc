namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class RealtimeCustomerNewPlaceBetClearCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "FromScannedTime")]
        public DateTime FromScannedTime { get; set; }

        [SpParam(SqlParamName = "ToScannedTime")]
        public DateTime ToScannedTime { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_Monitor_RealTime_ClearChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}