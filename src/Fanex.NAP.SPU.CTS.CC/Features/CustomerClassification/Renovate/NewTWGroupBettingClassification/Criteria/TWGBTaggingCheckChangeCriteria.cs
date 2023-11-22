namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NewTWGroupBettingClassification.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TWGBTaggingCheckChangeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "NextScannedDateTime")]
        public DateTime NextScannedDateTime { get; set; }

        public override string GetSettingKey() => "CTS_GeneralNormalClassification_TWGroupBetting_GetChanges";

        public override bool IsValid() => true;
    }
}
