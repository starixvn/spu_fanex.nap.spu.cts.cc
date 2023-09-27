namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using System.Collections.Generic;

    public class ScannedCustModel
    {
        public IEnumerable<CustNullUserName2Model> CustList { get; set; }

        public long LastCTSCustID { get; set; }
    }
}