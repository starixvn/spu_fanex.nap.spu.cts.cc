namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateAssociationCustomerLastTicketCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfoJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Arc_CustomerAssociation_LastTicketDate_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
