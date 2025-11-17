namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;

    public class InsertProblemAccountBySportEntity : CustomerClassificationBySportEntity
    {      
        public InsertProblemAccountBySportEntity() { }

        public InsertProblemAccountBySportEntity(ClassifyGBMMCustInfosBySportEntity gbMMCust)
        {
            CTSCustID = gbMMCust.CTSCustID;
            CustID = gbMMCust.CustID;
            RoleID = gbMMCust.RoleID;
            SportGroup = gbMMCust.SportGroup;
            SubscriberID = gbMMCust.SubscriberID;
            CategoryID = gbMMCust.CategoryID;
            CategoryGroupID = gbMMCust.CategoryGroup;
            IsLicensee = gbMMCust.IsLicensee;
            MatchID = gbMMCust.MatchID;
            CreatedBy = Constants.StarixITId;
            Remark = gbMMCust.MatchID.ToString();
        }

        public Int64 CTSCustID { get; set; }

        public int? RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicensee { get; set; }

        public Int64? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? WinlossStatus { get; set; }

        public int SportGroup { get; set; }

        public int? MatchID { get; set; }

        public string Remark { get; set; }
    }
}
