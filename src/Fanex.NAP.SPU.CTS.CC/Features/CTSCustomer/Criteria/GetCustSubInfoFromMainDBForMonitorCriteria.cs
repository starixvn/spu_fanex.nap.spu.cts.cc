namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetCustSubInfoFromMainDBForMonitorCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_Monitoring_GetCustSubInfo";
        }

        public override bool IsValid()
        {
            return FromDate != DateTime.MinValue && ToDate != DateTime.MinValue;
        }
    }
}