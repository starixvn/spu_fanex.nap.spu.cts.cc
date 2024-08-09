using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts
{
    public struct CustClassificationToMainDBConsts
    {
        public struct PrivateMethodName
        {
            public const string PushCustClassificationToMainDB = "PushCustClassificationToMainDB";

            public const string IsPushExternalSuccess = "IsPushExternalSuccess";

            public const string GetLatestCCAndDangerLevel = "GetLatestCCAndDangerLevel";

            public const string UpdateDangerLevelToMainDB = "UpdateDangerLevelToMainDB";

            public const string PushDangerLevel = "PushDangerLevel";

            public const string GetCustomerClassificationBySports = "GetCustomerClassificationBySports";
        }
    }
}
