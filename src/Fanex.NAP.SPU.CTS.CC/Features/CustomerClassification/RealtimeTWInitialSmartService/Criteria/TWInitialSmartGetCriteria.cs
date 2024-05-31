namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWEarlyBlockService
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TWInitialSmartGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_NextScannedDateTime")]
        public DateTime NextScannedDateTime { get; set; }

        [SpParam(SqlParamName = "op_NextScannedCustID")]
        public long NextScannedCustID { get; set; }

        public override string GetSettingKey() => "CTS_DC_CustClassification_InitialSmart_Get";

        public override bool IsValid() => true;
    }
}
