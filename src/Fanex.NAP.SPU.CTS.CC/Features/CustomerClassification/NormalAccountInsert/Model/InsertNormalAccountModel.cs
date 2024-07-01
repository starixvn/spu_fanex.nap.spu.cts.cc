namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using System;

    public class InsertNormalAccountModel
    {
        public InsertNormalAccountModel(NormalAccountAfterCheckTWDetectionModel cust)
        {
            this.CustID = cust.CustId;         
            this.CategoryID = cust.CategoryId;
            this.CategoryGroupID = cust.CategoryGroupId;
            this.CreatedTime = cust.CreatedTime;
            this.TurnoverRM = cust.TurnoverRM;
            this.WinlossRM = cust.WinlossRM;
            this.BetCount = cust.BetCount;
            this.ActiveDays = cust.ActiveDays;
            this.PerformanceTime = cust.PerformanceTime;
            this.TaggingID = cust.TaggingID;
            this.TaggingType = cust.TaggingType;
            this.TWBetCount = cust.TWBetCount;
            this.TWDesktopUsageRate = cust.TWDesktopUsageRate;
            this.TWGroupBettingRate = cust.TWGroupBettingRate;
            this.TWTicketRejectRate = cust.TWTicketRejectRate;
            this.ScanTaggingType = cust.ScanTaggingType;         
        }

        public InsertNormalAccountModel(NormalAccountAfterCheckTWDetectionModel cust, CTSTaggedCustEntity ctsTaggedCust)
        {
            this.CustID = cust.CustId;        
            this.CategoryID = cust.CategoryId;
            this.CategoryGroupID = cust.CategoryGroupId;
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
            }

            this.TWBetCount = cust.TWBetCount;            
            this.TWDesktopUsageRate = cust.TWDesktopUsageRate;
            this.TWGroupBettingRate = cust.TWGroupBettingRate;
            this.TWTicketRejectRate = cust.TWTicketRejectRate;
            this.ScanTaggingType = cust.ScanTaggingType;
        }

        public long CustID { get; set; }     

        public int? CategoryID { get; set; }

        public int? CategoryGroupID { get; set; }

        public DateTime CreatedTime { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }

        public DateTime PerformanceTime { get; set; }

        public int TaggingID { get; set; }

        public int TaggingType { get; set; }

        public int TWBetCount { get; set; }
        
        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

        public int ScanTaggingType { get; set; }
    }
}
