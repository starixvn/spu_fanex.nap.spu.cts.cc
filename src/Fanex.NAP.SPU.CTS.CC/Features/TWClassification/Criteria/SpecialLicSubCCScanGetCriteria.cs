namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class SpecialLicSubCCScanGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialLicSubCC_Scan_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
