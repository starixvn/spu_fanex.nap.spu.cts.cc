namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerCategoryRealtimeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Realtime_GetCategory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIdList);
        }
    }
}
