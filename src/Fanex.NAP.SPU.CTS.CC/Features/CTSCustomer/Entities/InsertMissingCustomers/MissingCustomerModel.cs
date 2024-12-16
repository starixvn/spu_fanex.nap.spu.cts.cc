namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using System;
    using System.Collections.Generic;

    public class MissingCustomerModel
    {
        public Int64? lastScannedID { get; set; }

        public IEnumerable<long> CustIDs { get; set; }
    }
}