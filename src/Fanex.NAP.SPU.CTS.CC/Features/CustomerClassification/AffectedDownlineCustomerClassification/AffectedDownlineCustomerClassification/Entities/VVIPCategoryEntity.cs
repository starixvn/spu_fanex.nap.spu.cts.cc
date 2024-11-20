namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline
{
    public class VVIPCategoryEntity
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int RoleID { get; set; }

        public int? CategoryID { get; set; }

        public int SubscriberID { get; set; }

        public int CreatedBy { get; set; }

        public string Remark { get; set; }
    }
}