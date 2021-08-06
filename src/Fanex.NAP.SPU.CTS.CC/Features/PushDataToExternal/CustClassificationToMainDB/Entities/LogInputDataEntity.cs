namespace Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Newtonsoft.Json;

    public class LogInputDataEntity
    {
        [JsonProperty("CustID")]
        public long? CustId { get; set; }

        [JsonProperty("InputValue")]
        public object InputValue { get; set; }
    }
}
