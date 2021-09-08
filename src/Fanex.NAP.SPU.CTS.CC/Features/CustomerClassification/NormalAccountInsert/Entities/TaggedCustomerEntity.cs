namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    public class TaggedCustomerEntity
    {
        public int CustID { get; set; }

        public int TaggingID { get; set; }

        public int TaggingType { get; set; }

        public int TWBetCount { get; set; }

        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }
    }
}