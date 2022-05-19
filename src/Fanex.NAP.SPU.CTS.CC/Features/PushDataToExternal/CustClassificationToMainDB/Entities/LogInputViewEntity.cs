namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using System.Collections.Generic;

    public class LogInputViewEntity
    {
        public IEnumerable<RobotInsetViewEntity> RobotUser { get; set; }
    }
}