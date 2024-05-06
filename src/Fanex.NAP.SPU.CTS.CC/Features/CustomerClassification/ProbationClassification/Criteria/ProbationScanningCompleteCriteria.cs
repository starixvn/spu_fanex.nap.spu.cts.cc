namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProbationScanningCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_ProbationScan_UpdateScanDate";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustSportJson);
        }
    }
}