namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckNewGroupBettingCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custIdList")]
        public string CustIdList { get; set; }

        public override string GetSettingKey() => "CTS_CustomerClassification_CheckNewGroupBetting";

        public override bool IsValid() => true;
    }
}
