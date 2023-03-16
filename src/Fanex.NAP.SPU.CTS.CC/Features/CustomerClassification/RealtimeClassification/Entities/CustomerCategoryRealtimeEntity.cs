namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class CustomerCategoryRealtimeEntity
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("CategoryId")]
        public int CategoryId { get; set; }
    }
}