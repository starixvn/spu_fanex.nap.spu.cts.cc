namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities
{
    using System.Collections.Generic;

    public class ScanLatencyCustomerModel
    {
        public IEnumerable<ScanLatencyCustomerEntity> ScanLatencyCustomers { get; set; }
        
        public int? NextScannedCustID { get; set; }
    }
}
