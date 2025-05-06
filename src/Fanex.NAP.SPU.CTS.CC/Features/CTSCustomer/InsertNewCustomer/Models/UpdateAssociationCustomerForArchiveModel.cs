namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Models
{
    public class UpdateAssociationCustomerForArchiveModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int RoleID { get; set; }

        public int CustSubID { get; set; }
    }
}