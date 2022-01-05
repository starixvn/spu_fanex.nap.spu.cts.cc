namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Newtonsoft.Json;

    public class AccountDBResult
    {
        [JsonProperty(PropertyName = "StatusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}