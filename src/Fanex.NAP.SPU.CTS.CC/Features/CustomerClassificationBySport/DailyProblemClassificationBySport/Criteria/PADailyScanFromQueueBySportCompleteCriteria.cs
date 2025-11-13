namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PADailyScanFromQueueBySportCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfoList")]
        public string CustInfoJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassificationBySport_DailyScanPA_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
