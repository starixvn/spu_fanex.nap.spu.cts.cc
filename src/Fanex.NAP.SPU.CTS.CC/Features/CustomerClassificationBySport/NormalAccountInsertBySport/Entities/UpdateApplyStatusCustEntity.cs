namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{   
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class UpdateApplyStatusCustEntity
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("SportId")]
        public int SportId { get; set; }
       
    }
}
