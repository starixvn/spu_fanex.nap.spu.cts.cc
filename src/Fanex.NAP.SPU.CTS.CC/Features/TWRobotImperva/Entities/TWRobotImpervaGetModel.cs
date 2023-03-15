namespace Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Entities
{
    using System.Collections.Generic;
    using System.Linq;

    public class TWRobotImpervaGetModel
    {
        public TWRobotImpervaGetModel()
        {
            RobotInfos = Enumerable.Empty<TWRobotImpervaGetEntity>();
        }

        public IEnumerable<TWRobotImpervaGetEntity> RobotInfos { get; set; }

        public long MaxLogID { get; set; }
    }
}
