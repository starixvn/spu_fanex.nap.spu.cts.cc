namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TWGroupBetting_ScanTicketCriteria : CriteriaBase
    {
        public DateTime FromTime { get; set; }
        
        public DateTime ToTime { get; set; }

        public int Step { get; set; }

        public Int64 LastTransID { get; set; }

        public Int64 MaxTransID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_TWGroupBetting_ScanTicket";
        }

        public override bool IsValid()
        {
            return FromTime != DateTime.MinValue && ToTime != DateTime.MinValue;
        }
    }
}
