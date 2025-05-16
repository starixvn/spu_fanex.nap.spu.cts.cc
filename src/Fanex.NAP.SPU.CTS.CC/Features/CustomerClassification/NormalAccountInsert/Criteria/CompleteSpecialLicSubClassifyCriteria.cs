namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CompleteSpecialLicSubClassifyCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public string CustIDs { get; set; }

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