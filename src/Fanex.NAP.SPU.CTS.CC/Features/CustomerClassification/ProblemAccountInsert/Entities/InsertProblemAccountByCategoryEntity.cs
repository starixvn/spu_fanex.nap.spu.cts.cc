namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System;

    public class InsertProblemAccountByCategoryEntity
    {
        public Int64 CustID { get; set; }

        public Int64 CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsLicensee { get; set; }

        public Int64? CreatedBy { get; set; }

        public string Remark { get; set; }

        public bool IsDirectPA { get; set; }

        public int? WinlossStatus { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }
    }
}