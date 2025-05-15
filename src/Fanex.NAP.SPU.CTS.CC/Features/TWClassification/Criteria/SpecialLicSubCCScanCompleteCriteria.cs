namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SpecialLicSubCCScanCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_IDList")]
        public string IDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialLicSubCC_Scan_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }    
}
