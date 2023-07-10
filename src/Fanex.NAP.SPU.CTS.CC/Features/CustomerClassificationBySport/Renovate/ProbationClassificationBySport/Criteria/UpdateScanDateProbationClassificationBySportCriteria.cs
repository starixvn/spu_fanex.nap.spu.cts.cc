namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.ProbationClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateScanDateProbationClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_Renovate_ProbationScan_UpdateScanDate";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustSportJson);
        }
    }
}
