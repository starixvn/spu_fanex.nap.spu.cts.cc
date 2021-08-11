namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckGroupBettingAndRejectCriteria : CriteriaBase
    {
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerClassification_CheckGroupBettingAndReject";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIdList);
        }
    }
}