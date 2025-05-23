namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class GetCurrentNormalCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfos { get; set; }

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
