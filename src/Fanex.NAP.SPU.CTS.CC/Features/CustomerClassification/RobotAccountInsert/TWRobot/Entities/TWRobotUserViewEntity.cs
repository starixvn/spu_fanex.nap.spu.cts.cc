namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using System.Collections.Generic;
    using System.Linq;

    public class TWRobotUserViewEntity
    {
        public IEnumerable<TWRobotUserEntity> RobotUsers { get; set; }

        public long MaxTimestamp { get; set; }
    }
}