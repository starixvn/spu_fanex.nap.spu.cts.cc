namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class ReactivatedCustomerGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "LastScannedDate")]
        public DateTime LastScannedDate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_GetReactivated";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}