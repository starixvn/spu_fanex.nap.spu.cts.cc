namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities
{
    using System.Collections.Generic;

    public class ScanLatencyCustomerEntity
    {
        public IEnumerable<int> CustIDs { get; set; }
        
        public int NextScannedCustID { get; set; }
    }
}
