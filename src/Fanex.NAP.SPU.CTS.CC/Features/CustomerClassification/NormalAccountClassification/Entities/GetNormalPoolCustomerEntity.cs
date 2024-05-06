namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    public class GetNormalPoolCustomerEntity
    {
        public long CustId { get; set; }

        public bool IsRealtimeOnly { get; set; }

        public int ScanTaggingType { get; set; }

        public bool IsScanTaggingNewCust { get; set; }
    }
}
