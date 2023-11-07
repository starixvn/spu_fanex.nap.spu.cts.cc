namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NewTWGroupBettingClassification.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TWGBTaggingPreprocessCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustId")]
        public string ListCustId { get; set; }

        public override string GetSettingKey() => "CTS_GeneralNormalClassification_TWGroupBetting_Preprocess";

        public override bool IsValid() => true;
    }
}
