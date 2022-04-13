namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data.Repository;

    public class TWGroupBetting_CountGBTicketCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_TWGroupBetting_CountGBTicket";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}