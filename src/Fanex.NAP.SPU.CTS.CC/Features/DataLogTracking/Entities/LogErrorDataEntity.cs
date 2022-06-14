namespace Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking
{
    using Newtonsoft.Json;

    public class LogErrorDataEntity
    {
        [JsonProperty("SpName")]
        public string SPName { get; set; }

        [JsonProperty("Parameters")]
        public object Parameters { get; set; }

        [JsonProperty("Message")]
        public object Message { get; set; }

        [JsonProperty("FromFlow")]
        public string FromFlow { get; set; }
    }
}