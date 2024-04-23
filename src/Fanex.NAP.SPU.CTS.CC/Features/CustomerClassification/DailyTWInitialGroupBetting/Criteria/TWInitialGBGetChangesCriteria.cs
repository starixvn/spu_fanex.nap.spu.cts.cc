namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyTWInitialGroupBetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TWInitialGBGetChangesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "NextScannedDateTime")]
        public DateTime NextScannedDateTime { get; set; }

        [SpParam(SqlParamName = "NextScannedCustID")]
        public long NextScannedCustID { get; set; }

        public override string GetSettingKey() => "CTS_GeneralNormalClassification_TWGroupBetting_GetChanges";

        public override bool IsValid() => true;
    }
}
