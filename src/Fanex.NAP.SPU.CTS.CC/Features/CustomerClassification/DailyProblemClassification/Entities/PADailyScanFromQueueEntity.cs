namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification.Entities
{
    using Newtonsoft.Json;

    public class PADailyScanFromQueueEntity
    {
        public long CustID { get; set; }

        [JsonProperty("SportGroup")]
        public int DWSportType { get; set; }
    }
}
