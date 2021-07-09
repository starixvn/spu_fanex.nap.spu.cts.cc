namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.DailyClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustClassificationForDailyScanningCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustSport")]
        public string CustSportJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DailyScan_UpdateStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}