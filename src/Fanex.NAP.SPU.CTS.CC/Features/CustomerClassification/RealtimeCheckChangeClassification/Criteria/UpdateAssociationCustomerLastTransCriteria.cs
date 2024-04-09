namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateAssociationCustomerLastTransCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfos { get; set; }

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
