namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
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

    public class TVSProblemAccountServices : ProblemAccountServices, ITVSProblemAccountServices
    {
        private readonly ITVSProblemAccountDataAccess tvsProblemAccountDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        public TVSProblemAccountServices(
            ISentryClient sentryClient,
            ITVSProblemAccountDataAccess tvsProblemAccountDataAccess,
            IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ICustClassToMainDBService custClassToMainDBService) : base(baseDataAccess, problemAccountDataAccess, sentryClient)
        {
            this.tvsProblemAccountDataAccess = tvsProblemAccountDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public TVSProblemAccountResultEntity GetTVSProblemAccounts(DateTime? lastDateTime)
        {
            return tvsProblemAccountDataAccess.GetTWProblemAccount(lastDateTime) ?? new TVSProblemAccountResultEntity();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S121:Control structures should use curly braces", Justification = "<Pending>")]
        public bool InsertTVSProblemAccounts(IEnumerable<TVSProblemAccountEntity> problemAccounts, PushDataToExternalEntity parameters)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var insertedPA = problemAccounts.Where(c => c.ReasonID != (int)Reason.ABBet32
                                                       && c.ReasonID != (int)Reason.ABBet104
                                                       && c.ReasonID != (int)Reason.TVSRobot36
                                                       && c.ReasonID != (int)Reason.TVSRobot105).Select(c => new InsertProblemAccountEntity(c));

                if (!insertedPA.Any())
                {
                    return true;
                }

                var insertedPAChunks = insertedPA.Chunk(parameters.ExternalBatchSize);

                foreach (var insertedPABatch in insertedPAChunks)
                {
                    var result = InsertProblemAccountPackage(insertedPABatch, FunctionId.TVSProblemAccountInsert, CCInputFlow.InsertPAReason, false, parameters.ServiceName);

                    var isSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(result, parameters);

                    if (!isSuccess)
                        return isSuccess;
                }
            }

            return true;
        }
    }
}