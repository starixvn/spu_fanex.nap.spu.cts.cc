namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport
{
    using System;   

    public class GetNormalAccountBySportEntity
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
    }
}
