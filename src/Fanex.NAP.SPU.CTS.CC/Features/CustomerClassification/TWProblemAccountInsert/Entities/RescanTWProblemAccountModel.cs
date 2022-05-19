namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Entities
{
    using System;  

    public class RescanTWProblemAccountModel
    {
        public int CustID { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public int ReasonId { get; set; }

        public int PreventActionId { get; set; }

        public string Comment { get; set; }

        public string VoidTime { get; set; }

        public decimal LimitAmount { get; set; }

        public int UserId { get; set; }

        public string Ip { get; set; }

        public bool IsMonitorCategoryGroup { get; set; }

        public bool IsMonitorSite { get; set; }

        public decimal? Winloss { get; set; }

        public DateTime InsertTime { get; set; }
    }
}
