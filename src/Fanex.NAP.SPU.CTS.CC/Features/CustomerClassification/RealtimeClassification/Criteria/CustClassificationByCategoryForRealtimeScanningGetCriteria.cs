namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustClassificationByCategoryForRealtimeScanningGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportsJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_GetByCustIDsAndCategoryIDs";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}