namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PADailyScanCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DailyScanPA_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}