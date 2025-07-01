namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWInitialGroupBettingService
    {
        TWInitialGBGetChangesModel GetChanges();

        bool InsertTWInitalGBClassification(IEnumerable<TWInitialGBGetChangesEntity> custIDs, PushDataToExternalEntity paras);

        void UpdateNextScannedTime(DateTime nextScannedDateTime, long nextScannedCustID);
    }

    public class TWInitialGroupBettingService : ProblemAccountServices, ITWInitialGroupBettingService
    {
        private readonly ITWInitialGroupBettingDataAccess dataAccess;      
        private readonly ICustClassToMainDBService custClassToMainDBService;

        private readonly string serviceName;     

        public TWInitialGroupBettingService(IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ICustClassToMainDBService custClassToMainDBService,
            ITWInitialGroupBettingDataAccess dataAccess) : base(baseDataAccess, problemAccountDataAccess)
        {
            this.dataAccess = dataAccess;          
            this.custClassToMainDBService = custClassToMainDBService;
            this.serviceName = "TWInitialGroupBettingService";
        }

        public TWInitialGBGetChangesModel GetChanges()
        {
            return dataAccess.GetChanges();
        }

        public void UpdateNextScannedTime(DateTime nextScannedDateTime, long nextScannedCustID)
        {
            dataAccess.UpdateNextScannedTime(nextScannedDateTime, nextScannedCustID);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S927:Parameter names should match base declaration and other partial definitions", Justification = "<Pending>")]
        public bool InsertTWInitalGBClassification(IEnumerable<TWInitialGBGetChangesEntity> twGBCusts, PushDataToExternalEntity paras)
        {
            var custInfos = dataAccess.GetCustInfos(string.Join(",", twGBCusts.Select(x => x.CustID)));
           
            var pas = twGBCusts.Join(custInfos, twGBCust => twGBCust.CustID, ctsCust => ctsCust.CustID,
                                (twGBCust, ctsCust) => new InsertProblemAccountEntity(twGBCust, ctsCust));

            var pasChunks = pas.Chunk(paras.ExternalBatchSize);

            foreach (var pasChunk in pasChunks)
            {                
                var paCustIDs = InsertProblemAccountPackage(pasChunk, FunctionId.TWInitialGBClassification, CCInputFlow.Potential, false, serviceName);

                custClassToMainDBService.PushExternalPackageByCustId(paCustIDs, paras);
            }

            return true;
        }
    }
}
