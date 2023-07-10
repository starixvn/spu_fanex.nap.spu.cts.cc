namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.ProbationClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetProbationScanClassificationBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int NumberOfRecord { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_Renovate_ProbationScan_Get";
        }

        public override bool IsValid()
        {
            return NumberOfRecord > 0;
        }
    }
}
