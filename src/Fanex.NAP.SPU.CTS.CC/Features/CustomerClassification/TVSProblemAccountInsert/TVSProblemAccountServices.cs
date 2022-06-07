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
                var insertedPA = problemAccounts.Where(c=>c.ReasonID != (int)Reason.Abnormal && c.ReasonID != (int)Reason.AbnormalM).Select(c => new InsertProblemAccountEntity
                {
                    CustID = c.CustID,
                    ReasonID = c.ReasonID,
                    CreatedBy = c.CreatedBy,
                    TVSRequestID = c.RequestID,
                    IsFromTVS = true,
                    IsDirectPA = true
                });

                var result = problemAccountServices.InsertProblemAccountPackage(insertedPA, FunctionId.TVSProblemAccountInsert, InsertPA_ByType.Reason, false, parameters.ServiceName);

                isSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(result, parameters);
            }

            return isSuccess;
        }
    }
}