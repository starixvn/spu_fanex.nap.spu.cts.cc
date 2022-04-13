namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data.Repository;

    public class TWGroupBetting_SyncDataGroupBettingTicket : NonQueryCommand
    {
        public string GBTicketXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_TWGroupBetting_SyncDataGroupBettingTicket";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(GBTicketXML);
        }
    }
}