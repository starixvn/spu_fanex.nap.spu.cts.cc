namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System;

    public class ExistedProblemAccountEntity
    {
        public Int64 CTSCustID { get; set; }

        public Int64 CustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicensee { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsDirect { get; set; }

        public bool IsRobot { get; set; }

        public bool IsFromTVS { get; set; }

        public int? TVSRequestID { get; set; }

        public bool? IsParlay { get; set; }

        public int? SportType { get; set; }

        public int? IssueTypeID { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromImperva { get; set; }        

        public DateTime LastModifiedDate { get; set; }

        public string Remark { get; set; }
    }
}