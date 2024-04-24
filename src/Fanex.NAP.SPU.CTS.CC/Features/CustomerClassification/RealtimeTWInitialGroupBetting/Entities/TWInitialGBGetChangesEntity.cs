namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;

    public class TWInitialGBGetChangesEntity
    {
        public long CustID { get; set; }

        public DateTime DetectedDate { get; set; }  

        public int? GBTicketCount { get; set; }

        public int? SportType { get; set; }

        public string SportName
        {
            get
            {
                if (this.SportType != null)
                {
                    return CachedRef.GetSportTypeName(this.SportType.Value);
                }

                return Constants.TWInitialGBMissSport;
            }
        }
    }
}
