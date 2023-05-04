namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using System;  

    public class NormalAccountAfterCheckTWDetectionModel
    {
        public NormalAccountAfterCheckTWDetectionModel(GetNormalAccountEntity cust)
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
        }

        public NormalAccountAfterCheckTWDetectionModel(GetNormalAccountEntity cust, TWTaggedCustEntity tWTaggedCust)
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
            
            if (tWTaggedCust != null)
            {
                this.TaggingID = tWTaggedCust.TaggingID;
                this.TaggingType = tWTaggedCust.TaggingType;
                this.TWBetCount = tWTaggedCust.TWBetCount;
                this.TWDesktopUsageRate = tWTaggedCust.TWDesktopUsageRate;
                this.TWGroupBettingRate = tWTaggedCust.TWGroupBettingRate;
                this.TWTicketRejectRate = tWTaggedCust.TWTicketRejectRate;
            }
        }

        public long CustId { get; set; }

        public int SportGroupId { get; set; }

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

        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

    }
}
