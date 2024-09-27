namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateSMAAssociationCustomerLastTransCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string SMACustInfos { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Arc_CustomerAssociationSMA_LastTicketDate_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
