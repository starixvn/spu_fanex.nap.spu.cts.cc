namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
{
    using System;

    public class CustClassificationFromQueueEntity
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        //VVIP
        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public int CreatedBy { get; set; }

        public string Remark { get; set; }

        //PA
        public int? CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsLicensee { get; set; }

        public bool IsDirectPA { get; set; }

        public int? ReasonID { get; set; }

        public bool IsExceptDirectDownline { get; set; }

        //Winloss info
        public int? WinlossStatus { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromTVS { get; set; }

        public int TVSRequestID { get; set; }

        public bool IsFromTW { get; set; }

        //True if only removing SpecialCC
        public bool IsDataChanged { get; set; }
    }
}