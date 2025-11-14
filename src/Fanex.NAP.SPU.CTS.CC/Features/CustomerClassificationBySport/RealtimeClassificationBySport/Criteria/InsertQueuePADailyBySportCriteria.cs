namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertQueuePADailyBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_DailyScanPA_InsertToQueue";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
