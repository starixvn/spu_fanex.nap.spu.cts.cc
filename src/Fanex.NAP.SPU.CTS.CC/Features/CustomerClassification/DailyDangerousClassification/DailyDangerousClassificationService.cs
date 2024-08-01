namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification
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

    public interface IDailyDangerousClassificationService
    {
        DangerousToCCModel GetDangerousClassification(bool isLicensee);

        bool InsertDangerousClassification(IEnumerable<DangerousToCCEntity> dangerousCCs, bool isLicensee, PushDataToExternalEntity paras);
    }

    public class DailyDangerousClassificationService : ProblemAccountServices, IDailyDangerousClassificationService
    {
        private readonly IDailyDangerousClassificationDataAccess dangerousClassificationDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly string serviceName;

        public DailyDangerousClassificationService(
            IDailyDangerousClassificationDataAccess dangerousClassificationDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            IBaseCustomerClassificationDataAccess baseDataAccess,
            ICustClassToMainDBService custClassToMainDBService,
            ISentryClient sentryClient) : base(baseDataAccess, problemAccountDataAccess, sentryClient)
        {
            this.dangerousClassificationDataAccess = dangerousClassificationDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
            this.serviceName = "DailyDangerousClassificationService";
        }

        public DangerousToCCModel GetDangerousClassification(bool isLicensee)
        {
            return dangerousClassificationDataAccess.GetDangerousClassification(isLicensee);
        }

        public bool InsertDangerousClassification(IEnumerable<DangerousToCCEntity> dangerousCCs, bool isLicensee, PushDataToExternalEntity paras)
        {
            if (dangerousCCs != null && dangerousCCs.Any())
            {
                var pas = dangerousCCs.Select(x =>
                {
                    return new InsertProblemAccountEntity
                    {
                        CustID = x.CustID,
                        CTSCustID = x.CTSCustID,
                        RoleID = x.RoleID,
                        SubscriberID = x.SubscriberID,
                        CategoryID = x.CategoryID,
                        CategoryGroupID = x.CategoryGroup,
                        IsLicensee = isLicensee,
                        CreatedBy = StarixITId,
                        Remark = string.Empty,
                        IsDirectPA = null,
                        IsFromAI = true,
                        IsFromCTS = false,
                        IsFromTVS = false,
                        IsFromTW = false,
                    };
                });

                var pasChunks = pas.Chunk(paras.ExternalBatchSize);

                foreach (var pasChunk in pasChunks)
                {
                    var paCustIDs = InsertProblemAccountPackage(pasChunk, FunctionId.DailyDangerousClassification, CCInputFlow.Potential, false, serviceName);

                    custClassToMainDBService.PushExternalPackageByCustId(paCustIDs, paras);
                }
            }

            return true;
        }
    }
}