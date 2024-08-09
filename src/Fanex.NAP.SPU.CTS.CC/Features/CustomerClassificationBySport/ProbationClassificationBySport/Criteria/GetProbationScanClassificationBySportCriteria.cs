namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetProbationScanClassificationBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_LastCustID")]
        public long LastCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_ProbationScan_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
