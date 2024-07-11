namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using System;

    public class InsertCustomerClassificationEntity
    {
        public long CustID { get; set; }

        public int? CategoryID { get; set; }

        public int? CategoryGroupID { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        public DateTime? PerformanceTime { get; set; }
    }
}
