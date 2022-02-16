namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using System.Collections.Generic;

    public class RobotUserViewEntity
    {
        public IEnumerable<RobotUserEntity> RobotUsers { get; set; }

        public long MaxTimestamp { get; set; }
    }
}