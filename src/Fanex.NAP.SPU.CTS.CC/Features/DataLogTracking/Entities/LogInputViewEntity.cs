namespace Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using System.Collections.Generic;

    public class LogInputViewEntity
    {
        public IEnumerable<TWRobotUserEntity> RobotUser { get; set; }
    }
}