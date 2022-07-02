namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;

    public class InsertProblemAccountEntity
    {
        public InsertProblemAccountEntity()
        {
            IsFromTVS = false;
            IsFromCTS = false;
            IsFromTW = false;
            IsDataChanged = false;
        }

        public InsertProblemAccountEntity(ClassifyGBMMCustInfosEntity gbMMCust)
        {
            this.CTSCustID = gbMMCust.CTSCustID;
            this.CustID = gbMMCust.CustID;
            this.RoleID = gbMMCust.RoleID;
            this.SubscriberID = gbMMCust.SubscriberID;
            this.CategoryID = gbMMCust.CategoryID;
            this.CategoryGroup = gbMMCust.CategoryGroup;
            this.IsLicensee = gbMMCust.IsLicensee;
            this.MatchID = gbMMCust.MatchID;
            this.CreatedBy = Constants.StarixITId;
            this.Remark = gbMMCust.MatchID.ToString();
            this.IsDirectPA = false;
            this.IsFromCTS = true;
            this.IsFromTVS = false;
            this.IsFromTW = false;
            this.IsDataChanged = false;            
        }

        public Int64 CustID { get; set; }

        public Int64 CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public int? CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public int? ReasonID { get; set; }

        public bool IsLicensee { get; set; }

        public Int64? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Remark { get; set; }

        public bool IsDirectPA { get; set; }

        public int? WinlossStatus { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromTVS { get; set; }

        public int? TVSRequestID { get; set; }

        public bool IsFromTW { get; set; }

        //True if only removing SpecialCC
        public bool IsDataChanged { get; set; }

        public int? MatchID { get; set; }
    }
}