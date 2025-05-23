namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class SpecialLicSubCCResponseCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ResponseInfo")]
        public string ResponseInfo { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialLicSubCC_Response_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
