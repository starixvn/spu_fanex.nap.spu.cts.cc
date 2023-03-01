namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class RealtimeCustomerNewPlaceBetGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "FromScannedTime")]
        public DateTime FromScannedTime { get; set; }

        [SpParam(SqlParamName = "ToScannedTime")]
        public DateTime ToScannedTime { get; set; }

        [SpParam(SqlParamName = "op_MaxScannedID")]
        public long MaxScannedID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_Monitor_RealTime_GetChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}