namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using System;

    public class NormalAccountAfterCheckTWDetectionModel
    {
        public NormalAccountAfterCheckTWDetectionModel(GetNormalAccountEntity cust)
        {
            MappingGetNormalAccount(cust);
        }

        public NormalAccountAfterCheckTWDetectionModel(GetNormalAccountEntity cust, TWTaggedCustEntity tWTaggedCust)
        {
            MappingGetNormalAccount(cust);

            if (tWTaggedCust != null && tWTaggedCust.TaggingID != 0)
            {
                this.TaggingID = tWTaggedCust.TaggingID;
                this.TaggingType = tWTaggedCust.TaggingType;
                this.TWBetCount = tWTaggedCust.TWBetCount;
                this.TWDesktopUsageRate = tWTaggedCust.TWDesktopUsageRate;
                this.TWGroupBettingRate = tWTaggedCust.TWGroupBettingRate;
                this.TWTicketRejectRate = tWTaggedCust.TWTicketRejectRate;
            }
        }

        public NormalAccountAfterCheckTWDetectionModel(GetNormalAccountEntity cust, NewTWTaggedCustEntity twGBTaggingNewCust)
        {
            MappingGetNormalAccount(cust);

            if (twGBTaggingNewCust != null && twGBTaggingNewCust.TaggingID != 0)
            {
                this.TaggingID = twGBTaggingNewCust.TaggingID;
                this.TaggingType = twGBTaggingNewCust.TaggingType;              
                this.SportType = twGBTaggingNewCust.SportType;
            }
        }

        private void MappingGetNormalAccount(GetNormalAccountEntity cust)
        {
            this.CustId = cust.CustId;
            //this.SportGroupId = cust.SportGroupId;
            this.CategoryId = cust.CategoryId;
            this.CreatedTime = cust.CreatedTime;
            this.TurnoverRM = cust.TurnoverRM;
            this.WinlossRM = cust.WinlossRM;
            this.BetCount = cust.BetCount;
            this.ActiveDays = cust.ActiveDays;
            this.PerformanceTime = cust.PerformanceTime;
            this.ScanTaggingType = cust.ScanTaggingType;
        }

        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int? SportType { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedTime { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }

        public DateTime PerformanceTime { get; set; }

        public int TaggingID { get; set; }

        public int TaggingType { get; set; }

        public int TWBetCount { get; set; }

        public short? First5TWGBTicketCount { get; set; }

        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

        public int ScanTaggingType { get; set; }
    }
}
