namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Model;
    using System;

    public class InsertNormalAccountEntity : CustomerClassificationEntity
    {
        public InsertNormalAccountEntity(ProcessTaggingModel cust)
        {
            CustID = cust.CustId;
            CategoryID = cust.CategoryId;
            CategoryGroupID = cust.CategoryGroupId;
            CreatedTime = cust.CreatedTime;
            TurnoverRM = cust.TurnoverRM;
            WinlossRM = cust.WinlossRM;
            BetCount = cust.BetCount;
            ActiveDays = cust.ActiveDays;
            PerformanceTime = cust.PerformanceTime;
            TaggingID = cust.TaggingID;
            TaggingType = cust.TaggingType;
            TWBetCount = cust.TWBetCount;
            TWDesktopUsageRate = cust.TWDesktopUsageRate;
            TWGroupBettingRate = cust.TWGroupBettingRate;
            TWTicketRejectRate = cust.TWTicketRejectRate;
            ScanTaggingType = cust.ScanTaggingType;
            ScanSpecialLicSubType = cust.ScanSpecialLicSubType;
        }

        public DateTime CreatedTime { get; set; }

        public int TaggingID { get; set; }

        public int TaggingType { get; set; }

        public int TWBetCount { get; set; }

        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

        public int ScanTaggingType { get; set; }

        public int ScanSpecialLicSubType { get; set; }
    }
}
