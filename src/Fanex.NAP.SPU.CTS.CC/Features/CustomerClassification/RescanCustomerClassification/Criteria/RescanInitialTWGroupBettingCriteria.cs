namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RescanInitialTWGroupBettingCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custIdList")]
        public string CustIDs { get; set; }

        public override string GetSettingKey() => "CTS_CustomerClassification_CheckNewGroupBetting";

        public override bool IsValid() => true;
    }
}
