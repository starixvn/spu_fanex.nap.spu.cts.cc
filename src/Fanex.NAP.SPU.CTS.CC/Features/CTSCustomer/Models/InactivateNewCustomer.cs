namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models
{
    using System;
    using System.Collections.Generic;

    public class InactivateNewCustomer
    {
        public IEnumerable<long> CustIDs { get; set; }

        public DateTime LastCreatedDate { get; set; }

        public long LastCTSCustID { get; set; }

    }
}
