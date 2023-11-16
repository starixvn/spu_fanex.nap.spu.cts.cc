namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert.Entities
{
    public class NewTWTaggedCustEntity
    {
        public int CustID { get; set; }

        public int TaggingID { get; set; }

        public int TaggingType { get; set; }

        public short? First5TWGBTicketCount { get; set; }

        public int? SportType { get; set; }
    }
}
