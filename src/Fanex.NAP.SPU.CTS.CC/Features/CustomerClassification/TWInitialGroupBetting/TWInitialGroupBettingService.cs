namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWInitialGroupBetting
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWInitialGroupBettingService
    {
        TWInitialGBGetChangesModel GetChanges();

        bool InsertTWInitalGBClassification(IEnumerable<TWInitialGBGetChangesEntity> custIDs, PushDataToExternalEntity paras);

        void UpdateNextScannedTime(DateTime nextScannedDateTime);
    }

    public class TWInitialGroupBettingService : ITWInitialGroupBettingService
    {
        private readonly ITWInitialGroupBettingDataAccess dataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly string serviceName;

        public TWInitialGroupBettingService(IProblemAccountServices problemAccountServices, 
            ICustClassToMainDBService custClassToMainDBService,
            ITWInitialGroupBettingDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            this.problemAccountServices = problemAccountServices;
            this.dataAccess = dataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
            this.serviceName = "TWInitialGroupBettingService";
        }

        public TWInitialGBGetChangesModel GetChanges()
        {
            return dataAccess.GetChanges();
        }

        public void UpdateNextScannedTime(DateTime nextScannedDateTime)
        {
            dataAccess.UpdateNextScannedTime(nextScannedDateTime);
        }

        public bool InsertTWInitalGBClassification(IEnumerable<TWInitialGBGetChangesEntity> twGBCusts, PushDataToExternalEntity paras)
        {
            var custInfos = dataAccess.GetCustInfos(twGBCusts.Select(x => x.CustID));

            var pas = twGBCusts.Join(custInfos, twGBCust => twGBCust.CustID, ctsCust => ctsCust.CustID,
                                (twGBCust, ctsCust) => new InsertProblemAccountEntity {
                                    CustID = twGBCust.CustID,
                                    CTSCustID = ctsCust.CTSCustID,
                                    RoleID = ctsCust.RoleID,
                                    SubscriberID = ctsCust.SubscriberID,
                                    CategoryID = ctsCust.CategoryID,
                                    CategoryGroup = ctsCust.CategoryGroup,
                                    IsLicensee = ctsCust.IsLicensee,
                                    IsFromAI = false,
                                    IsDirectPA = null,
                                    IsFromCTS = false,
                                    IsFromTVS = false,
                                    IsFromTW = true,
                                    CreatedBy = StarixITId,
                                    Remark = $"Detection Date: { twGBCust.DetectionDate.ToString(CONVERT_TO_DATETIME_MMMddyyyy)} - GB Bet Count: {twGBCust.GBBetCount}"
                                });

            var pasChunks = pas.Chunk(paras.ExternalBatchSize);

            foreach (var pasChunk in pasChunks)
            {
                var paCustIDs = problemAccountServices.InsertProblemAccountPackage(pasChunk, FunctionId.DailyDangerousClassification, InsertPA_ByType.Category, false, serviceName);

                custClassToMainDBService.PushDataToExternal(paCustIDs, paras);
            }

            return true;
        }
    }
}
