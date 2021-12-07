using System.Text.Json.Serialization;

namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    public class CheckAccountExistEntity
    {

        public string AccountID { get; set; }

        [JsonIgnore]
        public string Status { get; set; }
    }
}