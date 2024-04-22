namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data.Repository;

    public class TWGroupBetting_UpdateTotalTicketCriteria : NonQueryCommand
    {
        public int BatchSize { get; set; }

        public long LastSequenceId { get; set; }

        public long LastSequenceId_New { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_TWGroupBetting_UpdateTotalTicket";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}