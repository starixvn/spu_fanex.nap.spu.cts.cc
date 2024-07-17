namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCategoryByCategoryTypeNameCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CategoryTypeName")]
        public string CategoryTypeName { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Category_GetByCategoryTypeName";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
