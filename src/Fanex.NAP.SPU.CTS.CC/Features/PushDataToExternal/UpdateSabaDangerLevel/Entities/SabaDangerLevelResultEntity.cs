namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SabaDangerLevelResultEntity
    {
        public SabaDangerLevelResultEntity()
        {
            CustSabaDangerLevel = Enumerable.Empty<SabaDangerLevelEntity>();
        }

        public IEnumerable<SabaDangerLevelEntity> CustSabaDangerLevel { get; set; }

        public UInt64 LastHistoryID { get; set; }

        public SabaDangerLevelUpdateResultEntity UpdatedResult { get; set; }
    }
}