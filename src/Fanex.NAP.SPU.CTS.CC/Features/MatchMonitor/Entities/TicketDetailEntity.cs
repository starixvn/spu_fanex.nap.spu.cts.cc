namespace Fanex.NAP.SPU.CTS.Features.MatchMonitor.Entities
{
    using Newtonsoft.Json;
    using System;

    public class TicketDetailEntity
    {
        [JsonProperty("TransID")]
        public Int64 TransId { get; set; }

        public DateTime TransDate { get; set; }

        [JsonProperty("MatchID")]
        public Int64 MatchId { get; set; }

        [JsonProperty("HomeID")]
        public Int64 HomeId { get; set; }

        [JsonProperty("AwayID")]
        public Int64 AwayId { get; set; }

        [JsonProperty("LeagueID")]
        public Int64 LeagueId { get; set; }

        public DateTime EventDate { get; set; }

        [JsonProperty("EventStatus")]
        public string EventStatus { get; set; }

        [JsonProperty("KickOffTime")]
        public DateTime KickOffTime { get; set; }

        [JsonProperty("Bettype")]
        public int BetType { get; set; }

        [JsonProperty("CustID")]
        public Int64 CustId { get; set; }

        [JsonIgnore]
        public string BetTeam { get; set; }

        public Decimal Stake { get; set; }

        [JsonIgnore]
        public Decimal Odds { get; set; }

        [JsonIgnore]
        public Decimal Hdp1 { get; set; }

        [JsonIgnore]
        public Decimal Hdp2 { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public bool LiveIndicator { get; set; }

        [JsonIgnore]
        public int TicketStatus { get; set; }
    }
}
