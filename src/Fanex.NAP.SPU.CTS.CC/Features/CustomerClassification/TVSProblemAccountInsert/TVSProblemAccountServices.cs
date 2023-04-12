namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITVSProblemAccountServices
    {
        TVSProblemAccountResultEntity GetTVSProblemAccounts(DateTime? lastDateTime);

        bool InsertTVSProblemAccounts(IEnumerable<TVSProblemAccountEntity> problemAccounts, PushDataToExternalEntity parameters);

        bool InsertTVSRobot(IEnumerable<TVSProblemAccountEntity> robots, PushDataToExternalEntity parameters);
    }

    public class TVSProblemAccountServices : ITVSProblemAccountServices
    {
        private readonly ISentryClient sentryClient;
        private readonly ITVSProblemAccountDataAccess tvsProblemAccountDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        public TVSProblemAccountServices(
            ISentryClient sentryClient,
            ITVSProblemAccountDataAccess tvsProblemAccountDataAccess,
            IProblemAccountServices problemAccountServices,
            ICustClassToMainDBService custClassToMainDBService)
        {
            this.sentryClient = sentryClient;
            this.tvsProblemAccountDataAccess = tvsProblemAccountDataAccess;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public TVSProblemAccountResultEntity GetTVSProblemAccounts(DateTime? lastDateTime)
        {
            return tvsProblemAccountDataAccess.GetTWProblemAccount(lastDateTime);
        }

        public bool InsertTVSProblemAccounts(IEnumerable<TVSProblemAccountEntity> problemAccounts, PushDataToExternalEntity parameters)
        {
            bool isSuccess = true;

            if (problemAccounts != null && problemAccounts.Any())
            {
                var insertedPA = problemAccounts.Where(c => c.ReasonID != (int)Reason.Abnormal
                                                       && c.ReasonID != (int)Reason.AbnormalM
                                                       && c.ReasonID != (int)Reason.ABBet
                                                       && c.ReasonID != (int)Reason.TVSRobot36
                                                       && c.ReasonID != (int)Reason.TVSRobot105).Select(c => new InsertProblemAccountEntity(c));

                var result = problemAccountServices.InsertProblemAccountPackage(insertedPA, FunctionId.TVSProblemAccountInsert, InsertPA_ByType.Reason, true, parameters.ServiceName);

                isSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(result, parameters);
            }

            return isSuccess;
        }

        public bool InsertTVSRobot(IEnumerable<TVSProblemAccountEntity> robots, PushDataToExternalEntity parameters)
        {
            bool isSuccess = true;

            if (robots != null && robots.Any())
            {
                var insertRobot = robots.Where(c => c.ReasonID == (int)Reason.TVSRobot36 && c.ReasonID == (int)Reason.TVSRobot105).Select(c => new InsertRobotEntity(c));

                var result = problemAccountServices.InsertRobotAccountPackage(insertRobot, false);

                isSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(result?.Select(c => c.CustID)?.Distinct(), parameters);

                if (isSuccess)
                {
                    isSuccess = problemAccountServices.CheckPAToUnflagBetlimit(result?.Select(c => c.CTSCustID)?.Distinct(), parameters.ServiceName);
                }
            }

            return isSuccess;
        }
    }
}