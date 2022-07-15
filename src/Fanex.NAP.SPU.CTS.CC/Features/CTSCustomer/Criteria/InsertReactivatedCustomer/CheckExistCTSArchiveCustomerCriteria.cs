namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckExistCTSArchiveCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }
      
        public override string GetSettingKey()
        {
            return "CTS_DC_ArchiveCustomer_Reactivate_CheckExist";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDs);
        }
    }
}
