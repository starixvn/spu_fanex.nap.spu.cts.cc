namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class CompareCustomerDataForMonitorCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerListJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_Monitoring_CompareData";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerListJson);
        }
    }
}