namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCurrentNormalCategoryBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_GetCurrentCategory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
