namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities
{
    using Newtonsoft.Json;
    using System;

    public class ScanLatencyCustomerEntity
    {
        public int CustID { get; set; }

        [JsonProperty("CustomerRepUpdateTime")]
        public DateTime? RepUpdateTime { get; set; }
    }
}
