namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustClassificationForDailyScanningGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int RecordQuantityPerBatch { get; set; }

        [SpParam(SqlParamName = "ip_Batch")]
        public int BatchQuantity { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DailyScan_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}