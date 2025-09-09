namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TWInitialSmartGetBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_NextScannedDateTime")]
        public DateTime NextScannedDateTime { get; set; }

        [SpParam(SqlParamName = "op_NextScannedCustID")]
        public long NextScannedCustID { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize")]
        public int InternalBatchSize { get; set; }

        public override string GetSettingKey() => "CTS_DC_CustClassification_InitialSmart_BySport_Get";

        public override bool IsValid() => true;
    }
}
