namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.RealtimeClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerCategoryRealtimeBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportsJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_Realtime_GetCategory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
