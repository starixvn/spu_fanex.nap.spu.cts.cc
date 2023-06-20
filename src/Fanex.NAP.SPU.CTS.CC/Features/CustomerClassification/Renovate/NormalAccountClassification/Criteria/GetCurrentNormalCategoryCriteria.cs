namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class GetCurrentNormalCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Normal_GetCurrentCategory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
