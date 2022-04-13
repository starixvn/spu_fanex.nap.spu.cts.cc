namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence
{
    using System;

    public class CustEvidenceInsertEntity
    {
        public long CustId { get; set; }

        public long CTSCustId { get; set; }

        public int SubscriberId { get; set; }

        public int EvidenceId { get; set; }

        public int ActionType { get; set; }

        public int? WinlossStatus { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }
    }
}