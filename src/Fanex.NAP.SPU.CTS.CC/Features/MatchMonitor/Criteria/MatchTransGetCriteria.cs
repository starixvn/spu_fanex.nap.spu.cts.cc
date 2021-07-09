namespace Fanex.NAP.SPU.CTS.Features.MatchMonitor.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchTransGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "QueryType")]
        public int QueryType { get; set; }

        [SpParam(SqlParamName = "LastScannedTransID")]
        public Int64 LastScannedTransID { get; set; }

        [SpParam(SqlParamName = "BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ListTransId")]
        public string ListTransId { get; set; }

        [SpParam(SqlParamName = "EventDate")]
        public DateTime EventDate { get; set; }

        public override string GetSettingKey()
                => "CTS_Rpt_MatchMonitor_TicketDetails";

        public override bool IsValid()
            => true;
    }
}