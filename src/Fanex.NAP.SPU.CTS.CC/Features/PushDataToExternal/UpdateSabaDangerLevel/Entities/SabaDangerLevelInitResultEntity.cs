namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SabaDangerLevelInitResultEntity
    {
        public SabaDangerLevelInitResultEntity()
        {
            CustSabaDangerLevel = Enumerable.Empty<SabaDangerLevelEntity>();
            UpdatedResult = new SabaDangerLevelUpdateResultEntity();
        }

        public IEnumerable<SabaDangerLevelEntity> CustSabaDangerLevel { get; set; }

        public int LastCategoryID { get; set; }

        public Int64 LastCustID { get; set; }

        public SabaDangerLevelUpdateResultEntity UpdatedResult { get; set; }
    }
}