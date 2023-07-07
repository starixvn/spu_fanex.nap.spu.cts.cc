namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data.Repository;

    public class TWGroupBetting_UpdateTotalTicketCriteria : NonQueryCommand
    {
        private bool IsBackUp;

        public TWGroupBetting_UpdateTotalTicketCriteria(bool isBackUp)
        {
            IsBackUp = isBackUp;
        }

        public int BatchSize { get; set; }

        public long LastSequenceId { get; set; }
   
        public long LastSequenceId_New { get; set; }

        public override string GetSettingKey()
        {
            return IsBackUp ? "CTS_TWGroupBetting_UpdateTotalTicket" : "CTS_TWGroupBetting_UpdateTotalTicketDB062";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
