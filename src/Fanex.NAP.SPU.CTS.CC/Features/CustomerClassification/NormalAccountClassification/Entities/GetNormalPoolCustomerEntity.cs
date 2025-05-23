namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    public class GetNormalPoolCustomerEntity
    {
        public long CustId { get; set; }

        public bool IsRealtimeOnly { get; set; }

        public int ScanTaggingType { get; set; }

        public int ScanSpecialLicSubType { get; set; } // 0: Not Exist LicSub, 1: Exist LicSub
    }
}
