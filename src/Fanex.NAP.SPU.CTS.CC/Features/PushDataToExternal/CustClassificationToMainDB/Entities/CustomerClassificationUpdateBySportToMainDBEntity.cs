namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class CustomerClassificationUpdateBySportToMainDBEntity
    {
        [XName("id")]
        public long CustID { get; set; }

        [XName("s")]
        public int SportID { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

    }
}
