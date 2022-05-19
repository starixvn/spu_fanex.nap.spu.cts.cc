namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerPerformanceCriteria : CriteriaBase
    {
        public string CustIdList { get; set; }

        public override string GetSettingKey()
            => "CTS_CustomerClassification_GetPerformance";

        public override bool IsValid()
            => !string.IsNullOrEmpty(CustIdList);
    }
}
