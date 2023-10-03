namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using Newtonsoft.Json;
    using System;

    public class ReactivatedCustModel
    {
        [JsonProperty("CustID")]
        public Int64 CustID { get; set; }

        [JsonProperty("LogTime")]
        public DateTime LogTime { get; set; }
    }
}