namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class TWGroupBetting_UpdateTotalTicketDB062Criteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_TWGroupBetting_UpdateTotalTicketDB062";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}