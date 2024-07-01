namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProbationScanningGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int NumberOfRecord { get; set; }

        [SpParam(SqlParamName = "op_LastCustID")]
        public long LastCustID { get; set; }

        [SpParam(SqlParamName = "op_LastCategoryID")]
        public int LastCategoryID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_ProbationScan_Get";
        }

        public override bool IsValid()
        {
            return NumberOfRecord > 0;
        }
    }
}