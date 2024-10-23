namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.TWRobot
{
    using System.Collections.Generic;
    using System.Linq;

    public class InsertTWAgencyRobotModel
    {
        public InsertTWAgencyRobotModel()
        {
            Robots = Enumerable.Empty<TWRobotUserEntity>();
        }

        public IEnumerable<TWRobotUserEntity> Robots { get; set; }
    }
}
