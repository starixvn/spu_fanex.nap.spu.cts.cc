namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class TWGroupBetting_ScanTicketCriteria : CriteriaBase
    {

        public int BatchSize { get; set; }

        public int Step { get; set; }

        public Int64 LastTransID { get; set; }
        
        public Int64 LastTransSec { get; set; }
        
        public int SkipTransCount { get; set; }

        
        public Int64 LastTransID_New { get; set; }
        public Int64 LastTransSec_New { get; set; }
        public int SkipTransCount_New { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_TWGroupBetting_ScanTicket";
        }

        public override bool IsValid()
        {
            return BatchSize > 0 && Step > 0;
        }
    }
}