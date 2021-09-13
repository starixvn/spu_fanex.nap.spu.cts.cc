namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using System.Collections.Generic;

    public class LogInputViewEntity
    {
        public IEnumerable<RobotUserEntity> RobotUser { get; set; }

        public IEnumerable<ProblemAccountsEntity> ProblemAccounts { get; set; }
    }
}
