namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using System;

    public class ProblemAccountClassificationEntity
    {
        public Int64 CustID { get; set; }

        public bool IsLosing { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }
    }
}