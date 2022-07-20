namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using System;

    public class MatchesInfosEntity
    {
        public int MatchID { get; set; }

        public string MatchCode { get; set; }

        public int SportType { get; set; }

        public int LeagueId { get; set; }

        public DateTime EventDate { get; set; }

        public DateTime KickOffTime { get; set; }

        public int AwayId { get; set; }

        public int HomeId { get; set; }

        public string EventStatus { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public int FinalHomeScore { get; set; }

        public int FinalAwayScore { get; set; }
    }
}
