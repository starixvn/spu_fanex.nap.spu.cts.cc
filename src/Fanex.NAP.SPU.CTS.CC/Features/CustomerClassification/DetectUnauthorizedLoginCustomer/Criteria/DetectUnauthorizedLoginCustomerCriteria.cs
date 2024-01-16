namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class DetectUnauthorizedLoginCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ListCustJson")]
        public string CustJson { get; set; }

        [SpParam(SqlParamName = "ip_ProcessedDate")]
        public DateTime ProcessDate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DetectUnauthorizedLoginCustomer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
