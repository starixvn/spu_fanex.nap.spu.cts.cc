namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using System;

    public class InsertNormalAccountModel
    {
        public InsertNormalAccountModel(NormalAccountAfterCheckTWDetectionModel cust)
        {
            this.CustId = cust.CustId;
            this.SportGroupId = cust.SportGroupId;
            this.CategoryId = cust.CategoryId;
            this.CreatedTime = cust.CreatedTime;
            this.TurnoverRM = cust.TurnoverRM;
            this.WinlossRM = cust.WinlossRM;
            this.BetCount = cust.BetCount;
            this.ActiveDays = cust.ActiveDays;
            this.PerformanceTime = cust.PerformanceTime;
            this.TaggingID = cust.TaggingID;
            this.TaggingType = cust.TaggingType;
            this.TWBetCount = cust.TWBetCount;
            this.First5TWGBTicketCount = cust.First5TWGBTicketCount;
            this.TWDesktopUsageRate = cust.TWDesktopUsageRate;
            this.TWGroupBettingRate = cust.TWGroupBettingRate;
            this.TWTicketRejectRate = cust.TWTicketRejectRate;
            this.ScanTaggingType = cust.ScanTaggingType;
            this.SportType = cust.SportType;
        }

        public InsertNormalAccountModel(NormalAccountAfterCheckTWDetectionModel cust, CTSTaggedCustEntity ctsTaggedCust)
        {
            this.CustId = cust.CustId;
            this.SportGroupId = cust.SportGroupId;
            this.CategoryId = cust.CategoryId;
            this.CreatedTime = cust.CreatedTime;
            this.TurnoverRM = cust.TurnoverRM;
            this.WinlossRM = cust.WinlossRM;
            this.BetCount = cust.BetCount;
            this.ActiveDays = cust.ActiveDays;
            this.PerformanceTime = cust.PerformanceTime;

            if (ctsTaggedCust != null && cust.TaggingID == 0 && ctsTaggedCust.TaggingID != 0)
            {
                this.TaggingID = ctsTaggedCust.TaggingID;
                this.TaggingType = ctsTaggedCust.TaggingType;
            }
            else
            {
                this.TaggingID = cust.TaggingID;
                this.TaggingType = cust.TaggingType;
                this.SportType = cust.SportType;
            }

            this.TWBetCount = cust.TWBetCount;
            this.First5TWGBTicketCount = cust.First5TWGBTicketCount;
            this.TWDesktopUsageRate = cust.TWDesktopUsageRate;
            this.TWGroupBettingRate = cust.TWGroupBettingRate;
            this.TWTicketRejectRate = cust.TWTicketRejectRate;
            this.ScanTaggingType = cust.ScanTaggingType;
        }

        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int SportType { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedTime { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }

        public DateTime PerformanceTime { get; set; }

        public decimal LastXDaysMargin { get; set; }

        public decimal ProbationPeriodWinloss { get; set; }

        public int TaggingID { get; set; }

        public int TaggingType { get; set; }

        public int TWBetCount { get; set; }
        
        public short First5TWGBTicketCount { get; set; }

        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

        public int ScanTaggingType { get; set; }
    }
}
