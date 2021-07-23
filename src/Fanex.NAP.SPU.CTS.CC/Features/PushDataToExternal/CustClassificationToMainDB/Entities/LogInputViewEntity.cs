namespace Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RobotAccountInsert;
    using System.Collections.Generic;

    public class LogInputViewEntity
    {
        public IEnumerable<RobotUserEntity> RobotUser { get; set; }

        public IEnumerable<ProblemAccountsEntity> ProblemAccounts { get; set; }
    }
}
