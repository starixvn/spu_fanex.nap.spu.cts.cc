namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using System;
   
    public class GetNormalAccountEntity
    {
        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedTime { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }

        public DateTime PerformanceTime { get; set; }       
    
        public int ScanTaggingType { get; set; }

        public bool IsCheckTaggingTW { get; set; }
    }
}
