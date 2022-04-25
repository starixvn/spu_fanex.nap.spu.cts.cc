namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data.Repository;

    public class TWGroupBetting_CountGBTicketDB062Criteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_TWGroupBetting_CountGBTicketDB062";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}