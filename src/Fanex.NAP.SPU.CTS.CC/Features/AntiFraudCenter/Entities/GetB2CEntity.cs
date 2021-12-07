namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Newtonsoft.Json;
    using System.Runtime.Serialization;

    public class GetB2CEntity
    {
        [IgnoreDataMember]
        public int CustId { get; set; }

        [JsonProperty(PropertyName = "AccountID")]
        public string B2CCustId { get; set; }
    }
}