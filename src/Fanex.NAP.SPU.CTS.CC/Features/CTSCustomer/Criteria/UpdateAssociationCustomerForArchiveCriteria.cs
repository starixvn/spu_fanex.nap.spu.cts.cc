namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateAssociationCustomerForArchiveCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfoJson { get; set; }
     
        public override string GetSettingKey()
        {
            return "CTS_Arc_CustomerAssociation_InsertUpdate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
