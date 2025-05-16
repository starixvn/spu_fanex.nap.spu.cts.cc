namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Model
{
    using Fanex.NAP.SPU.CTS.CC.Shares;
    using System;

    public class ProcessTaggingModel : BaseCustTaggedModel
    {
        public ProcessTaggingModel() { }

        public ProcessTaggingModel(GetNormalAccountEntity cust)
        {
            MappingGetNormalAccount(cust);
        }

        public int? CategoryId { get; set; }

        public int? CategoryGroupId { get; set; }

        public DateTime CreatedTime { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }

        public DateTime PerformanceTime { get; set; }

        public int ScanTaggingType { get; set; }

        public int ScanTaggingTWType { get; set; }

        public int ScanSpecialLicSubType { get; set; }

        public int TaggingID { get; set; }

        public int TaggingType { get; set; }

        public int TWBetCount { get; set; }

        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

        public ProcessTaggingModel MergeTWTaggedCust(TWTaggedCustEntity tWTaggedCust)
        {
            if (tWTaggedCust.IsTWTaggedCustValid())
            {
                TaggingID = tWTaggedCust.TaggingID;
                TaggingType = tWTaggedCust.TaggingType;
                TWBetCount = tWTaggedCust.TWBetCount;
                TWDesktopUsageRate = tWTaggedCust.TWDesktopUsageRate;
                TWGroupBettingRate = tWTaggedCust.TWGroupBettingRate;
                TWTicketRejectRate = tWTaggedCust.TWTicketRejectRate;
            }

            return this;
        }

        public ProcessTaggingModel MergeCTSTaggedCust(CTSTaggedCustEntity ctsTaggedCust)
        {
            if (ctsTaggedCust.IsCTSTaggedCustValid(TaggingID))
            {
                TaggingID = ctsTaggedCust.TaggingID;
                TaggingType = ctsTaggedCust.TaggingType;
            }

            return this;
        }

        public ProcessTaggingModel MergeTWTaggedSpecialLicSubCust(CTSTaggedCustEntity tWTaggedCust)
        {
            if (tWTaggedCust.IsTWTaggedSpecialLicSubCustValid(TaggingID, ScanSpecialLicSubType))
            {
                TaggingID = tWTaggedCust.TaggingID;
                TaggingType = tWTaggedCust.TaggingType;
            }

            return this;
        }

        private void MappingGetNormalAccount(GetNormalAccountEntity cust)
        {
            CustId = cust.CustId;
            CategoryId = cust.CategoryId;
            CategoryGroupId = cust.CategoryGroupId;
            CreatedTime = cust.CreatedTime;
            TurnoverRM = cust.TurnoverRM;
            WinlossRM = cust.WinlossRM;
            BetCount = cust.BetCount;
            ActiveDays = cust.ActiveDays;
            PerformanceTime = cust.PerformanceTime;
            ScanTaggingType = cust.ScanTaggingType;
            ScanTaggingTWType = cust.ScanTaggingTWType;
            ScanSpecialLicSubType = cust.ScanSpecialLicSubType;
        }
    }
}
