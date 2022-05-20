namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System;

    public class InsertProblemAccountByReasonEntity
    {
        public Int64 CustID { get; set; }

        public int ReasonID { get; set; }

        public Int64? CreatedBy { get; set; }

        public int? WinlossStatus { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }
    }
}