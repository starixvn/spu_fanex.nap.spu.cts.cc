namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TWGBTaggingUpdateNextScannedTimeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "NextScannedDateTime")]
        public DateTime NextScannedDateTime { get; set; }

        public override string GetSettingKey() => "CTS_GeneralNormalClassification_TWGroupBetting_UpdateNextScannedTime";

        public override bool IsValid() => true;
    }
}
