namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using Newtonsoft.Json;
    using System;

    public class ProblemAccountLosingClassificationEntity
    {
        [JsonProperty("CustID")]
        public Int64 CustId { get; set; }

        public int WinlossStatus { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }

        public DateTime? PerformanceTime { get; set; }
    }
}