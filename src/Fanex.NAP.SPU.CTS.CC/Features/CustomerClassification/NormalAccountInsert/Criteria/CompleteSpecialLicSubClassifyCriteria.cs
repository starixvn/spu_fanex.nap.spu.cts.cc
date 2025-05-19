namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CompleteSpecialLicSubClassifyCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfo { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialLicSubCC_Classify_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}