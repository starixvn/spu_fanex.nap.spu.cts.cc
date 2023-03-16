namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustomerCategoryRealtimeGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Realtime_GetCategory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDs);
        }
    }
}