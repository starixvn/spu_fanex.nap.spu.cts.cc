namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using System;
    using System.Collections.Generic;

    public class RobotAIViewEntity
    {
        public IEnumerable<RobotAIEntity> RobotAIClassification { get; set; }

        public UInt64 LastScannedId { get; set; }
    }
}