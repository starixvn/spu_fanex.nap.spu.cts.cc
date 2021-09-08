namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TaggingByAssociationWithPAGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListCustID")]
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_GetTaggingByAssociationWithPA";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}