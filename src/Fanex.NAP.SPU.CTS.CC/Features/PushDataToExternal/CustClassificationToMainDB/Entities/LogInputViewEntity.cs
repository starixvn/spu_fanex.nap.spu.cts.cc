namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System.Collections.Generic;

    public class LogInputViewEntity
    {
        public IEnumerable<InsertRobotEntity> RobotUser { get; set; }
    }
}