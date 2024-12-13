namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMissingCustomerCriteria : CriteriaBase
    {

        [SpParam(SqlParamName = "op_LastScannedID")]
        public long LastScannedID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Monitoring_MissingCustomer_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}