namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using System;
    using System.Collections.Generic;

    public class ScannedCustModel
    {
        public IEnumerable<CustNullUserName2Model> CustList { get; set; }

        public UInt64 LastCTSCustID { get; set; }
    }
}