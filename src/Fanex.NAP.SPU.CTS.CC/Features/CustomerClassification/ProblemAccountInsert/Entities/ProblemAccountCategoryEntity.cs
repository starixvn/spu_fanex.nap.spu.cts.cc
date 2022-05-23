namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System;

    public class ProblemAccountCategoryEntity
    {
        public Int64 CTSCustID { get; set; }

        public Int64 CustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsLicensee { get; set; }

        public Int64? CreatedBy { get; set; }

        public string Remark { get; set; }

        public bool IsDirectPA { get; set; }
    }
}