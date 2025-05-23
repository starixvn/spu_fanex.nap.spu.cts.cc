namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification.Models
{
    using Newtonsoft.Json;

    public class SpecialLicSubCCResponseCompleteModel
    {
        public long ID { get; set; } 

        public int ResponseStatus { get; set; }

        public string ResponseTime { get; set; }

        public int ResponseHttpCode { get; set; }

        [JsonIgnore]
        public string Message { get; set; }
    }
}
