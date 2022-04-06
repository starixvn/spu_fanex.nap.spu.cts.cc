namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TWGroupBetting_GetRuleCriteria : CriteriaBase
    {

        public override string GetSettingKey()
        {
            return "CTS_TWGroupBetting_GetRules";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
