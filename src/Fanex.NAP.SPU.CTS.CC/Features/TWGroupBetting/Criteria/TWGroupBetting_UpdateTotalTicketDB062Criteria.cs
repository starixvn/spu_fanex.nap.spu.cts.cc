namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TWGroupBetting_UpdateTotalTicketCriteria : NonQueryCommand
    {
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
