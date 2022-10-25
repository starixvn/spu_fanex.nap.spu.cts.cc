namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class CustClassificationRealtimeEntity
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("SportGroupId")]
        public int SportGroupId { get; set; }

        [XName("CategoryId")]
        public int CurrentCategoryID { get; set; }
    }
}