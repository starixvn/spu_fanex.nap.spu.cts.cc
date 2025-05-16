namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckTWSpecialLicSubCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_GetTaggingBySpecialLicSubCC";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
