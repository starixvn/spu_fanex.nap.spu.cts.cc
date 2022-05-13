namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using System;

    public class InsertProblemAccountEntity
    {
        public long CustID { get; set; }

        public int ReasonID { get; set; }

        public int RobotCounter { get; set; }

        public Int64? CreatedBy { get; set; }

        public int? WinlossStatus { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }
    }
}