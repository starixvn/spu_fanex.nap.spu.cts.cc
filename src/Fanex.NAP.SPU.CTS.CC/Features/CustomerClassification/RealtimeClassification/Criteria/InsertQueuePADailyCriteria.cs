namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertQueuePADailyCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DailyScanPA_InsertToQueue";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
