namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerInfoFromCTSCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "ip_SourceType")]
        public int SourceType { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetByCustIDs";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIds);
        }
    }
}