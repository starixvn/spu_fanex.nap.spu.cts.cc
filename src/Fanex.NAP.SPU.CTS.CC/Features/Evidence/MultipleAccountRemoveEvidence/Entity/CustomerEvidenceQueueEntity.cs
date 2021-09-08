namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence
{
    using Newtonsoft.Json;

    public class CustomerEvidenceQueueEntity
    {
        [JsonIgnore]
        public long Id { get; set; }

        [JsonProperty("CTSCustID")]
        public long CTSCustId { get; set; }

        [JsonIgnore]
        public long CustId { get; set; }

        [JsonIgnore]
        public int SubscriberId { get; set; }

        [JsonProperty("EvidenceID")]
        public int EvidenceId { get; set; }

        public string Remark { get; set; }

        [JsonIgnore]
        public int ActionId { get; set; }

        public int CreatedBy { get; set; }
    }
}