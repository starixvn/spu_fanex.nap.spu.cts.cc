namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PADailyScanFromQueueBySportGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_DailyScanPA_GetQueue";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
