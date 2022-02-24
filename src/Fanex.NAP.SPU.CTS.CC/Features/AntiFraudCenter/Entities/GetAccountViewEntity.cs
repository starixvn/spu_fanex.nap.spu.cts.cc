namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class GetAccountViewEntity
    {
        [IgnoreDataMember]
        public int? CustId { get; set; }

        [JsonProperty(PropertyName = "AccountID")]
        public string AccountID { get; set; }

        [JsonProperty(PropertyName = "CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "Phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "BankAccountID")]
        public string BankAccountID { get; set; }

        [JsonProperty(PropertyName = "BankName")]
        public string BankName { get; set; }

        [JsonProperty(PropertyName = "FullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "FraudID")]
        public IEnumerable<int> FraudID { get; set; }
    }
}