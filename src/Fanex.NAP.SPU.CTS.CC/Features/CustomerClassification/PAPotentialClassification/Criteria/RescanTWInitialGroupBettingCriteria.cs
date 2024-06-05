namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.PAPotentialClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RescanTWInitialGroupBettingCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custIdList")]
        public string CustIDs { get; set; }

        public override string GetSettingKey() => "CTS_CustomerClassification_CheckNewGroupBetting";

        public override bool IsValid() => true;
    }
}
