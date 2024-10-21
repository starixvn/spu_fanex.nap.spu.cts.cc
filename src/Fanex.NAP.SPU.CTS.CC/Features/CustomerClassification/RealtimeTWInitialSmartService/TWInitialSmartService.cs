namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWInitialSmartService
    {
        TWInitialSmartModel GetCustInitialSmart();

        bool InsertTWInitialSmartClassification(IEnumerable<TWInitialSmartEntity> tWInitialSmartCusts, PushDataToExternalEntity paras);
    }

    public class TWInitialSmartService : ProblemAccountServices, ITWInitialSmartService
    {
        private readonly ITWInitialSmartDataAccess dataAccess;      
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly string serviceName;

        public TWInitialSmartService(IBaseCustomerClassificationDataAccess baseDataAccess, 
            IProblemAccountDataAccess problemAccountDataAccess,
            ISentryClient sentryClient,        
            ICustClassToMainDBService custClassToMainDBService,
            ITWInitialSmartDataAccess dataAccess) : base(baseDataAccess, problemAccountDataAccess, sentryClient)
        {
            this.dataAccess = dataAccess;        
            this.custClassToMainDBService = custClassToMainDBService;
            this.serviceName = "Realtime TW Initial Smart JobService";
        }

        public TWInitialSmartModel GetCustInitialSmart()
        {
            return dataAccess.GetCustInitialSmart();
        }

        public bool InsertTWInitialSmartClassification(IEnumerable<TWInitialSmartEntity> tWInitialSmartCusts, PushDataToExternalEntity paras)
        {
            var pas = tWInitialSmartCusts.Select(x => new InsertProblemAccountEntity(x));

            var pasChunks = pas.Chunk(paras.ExternalBatchSize);

            foreach (var pasChunk in pasChunks)
            {
                var paCustIDs = InsertProblemAccountPackage(pasChunk, FunctionId.TWInitialSmartClassification, CCInputFlow.Potential, false, serviceName);

                custClassToMainDBService.PushExternalPackageByCustId(paCustIDs, paras);
            }

            return true;
        }

    }
}
