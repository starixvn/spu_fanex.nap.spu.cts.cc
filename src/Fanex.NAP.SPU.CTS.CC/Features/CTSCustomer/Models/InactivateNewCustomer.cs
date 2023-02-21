namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using System;
    using System.Collections.Generic;

    public class InactivateNewCustomer
    {
        public IEnumerable<RescanCategoryDailyModel> CustInfos { get; set; }

        public UInt64 LastCustID { get; set; }

        public int LastCategoryID { get; set; }
    }
}