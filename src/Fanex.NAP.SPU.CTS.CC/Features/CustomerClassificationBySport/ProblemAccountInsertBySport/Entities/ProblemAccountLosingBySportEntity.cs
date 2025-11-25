namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities
{
    using System;
    using Newtonsoft.Json;

    public class ProblemAccountLosingBySportEntity
    {
        [JsonProperty("CustID")]
        public long CustId { get; set; }

        public int SportGroup { get; set; }

        public int WinlossStatus { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public long BetCount { get; set; }

        public int ActiveDays { get; set; }

        public DateTime? PerformanceTime { get; set; }
    }
}
