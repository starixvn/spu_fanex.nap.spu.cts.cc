namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using System;

    public class InsertNormalAccountEntity : CustomerClassificationEntity
    {
        public InsertNormalAccountEntity(NormalAccountAfterCheckTWDetectionModel cust)
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

        public InsertNormalAccountEntity(NormalAccountAfterCheckTWDetectionModel cust, CTSTaggedCustEntity ctsTaggedCust)
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

        public DateTime CreatedTime { get; set; }       

        public int TaggingID { get; set; }

        public int TaggingType { get; set; }

        public int TWBetCount { get; set; }
        
        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

        public int ScanTaggingType { get; set; }
    }
}
