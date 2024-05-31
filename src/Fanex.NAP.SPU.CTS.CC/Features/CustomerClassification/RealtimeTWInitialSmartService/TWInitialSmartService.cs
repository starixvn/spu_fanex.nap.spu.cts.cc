namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWEarlyBlockService
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWInitialSmartService
    {
        TWInitialSmartModel GetCustInitialSmart();

        bool InsertTWInitialSmartClassification(IEnumerable<TWInitialSmartEntity> tWInitialSmartCusts, PushDataToExternalEntity paras);
    }

    public class TWInitialSmartService : ITWInitialSmartService
    {
        private readonly ITWInitialSmartDataAccess dataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly string serviceName;

        public TWInitialSmartService(IProblemAccountServices problemAccountServices,
            ICustClassToMainDBService custClassToMainDBService,
            ITWInitialSmartDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            this.problemAccountServices = problemAccountServices;
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
                var paCustIDs = problemAccountServices.InsertProblemAccountPackage(pasChunk, FunctionId.TWInitialSmartClassification, InsertPA_ByType.Category, false, serviceName);

                custClassToMainDBService.PushDataToExternal(paCustIDs, paras);
            }

            return true;
        }

    }
}
