namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INormalAccountInsertBySportJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class NormalAccountInsertBySportJobService : INormalAccountInsertBySportJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly INormalAccountInsertBySportService normalAccountInsertService;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        private const int CategoryIdFromSpecialCC = -1;

        public NormalAccountInsertBySportJobService(ISentryClient sentryClient
            , INormalAccountInsertBySportService normalAccountInsertService
            , ICustClassToMainDBService custClassToMainDBService
            , IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.normalAccountInsertService = normalAccountInsertService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.serviceName = "InsertNormalAccountBySportRenovate";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                var isContinue = true;

                if (batchInternalSize <= 0)
                    batchInternalSize = 5000;

                do
                {
                    isContinue = JobRun(batchExternalSize, batchInternalSize);

                } while (isContinue);                
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(this.serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool JobRun(int batchExternalSize, int batchInternalSize)
        {
            var isContinue = false;

            var newClassifications = normalAccountInsertService.GetNormalAccount();

            if (newClassifications.NormalAccount.IsNotNullOrEmpty())
            {               
                var custChunks = newClassifications.NormalAccount.Chunk(batchInternalSize).ToList();

                custChunks
                    .AsParallel()
                    .WithDegreeOfParallelism(4)
                    .ForAll(custBatch => PrepareDataForInsertNormalAccount(custBatch, newClassifications.ScannedMaxId, batchExternalSize));
               
                isContinue = true;
            }

            return isContinue;
        }

        private void PrepareDataForInsertNormalAccount(IEnumerable<GetNormalAccountBySportEntity> custs, long scannedMaxId, int batchExternalSize)
        {
            var insertNormalAccounts    = custs.Where(x => x.CategoryId != CategoryIdFromSpecialCC);
            var specialCCAcounts        = custs.Where(x => x.CategoryId == CategoryIdFromSpecialCC);

            var resultInsertNormalAccount = InsertNormalAccount(insertNormalAccounts);

            if (IsInsertNormalAccountSuccess(resultInsertNormalAccount))
            {
                var isPushCCSuccess = PushCCBySportToMainDB(resultInsertNormalAccount?.CustNAInserted, specialCCAcounts, batchExternalSize);

                var customerXML = custs.Select(x => new CustSportXMLModel
                {
                    CustID = x.CustId,
                    SportGroup = x.SportGroup
                }).ToXmlString();

                normalAccountInsertService.CompleteNormalAccountBySport(customerXML, scannedMaxId, isPushCCSuccess);
            }
        }

        private bool IsInsertNormalAccountSuccess(InsertNormalAccountBySportModel resultInsertNormalAccount)
        {
            return resultInsertNormalAccount != null && string.IsNullOrEmpty(resultInsertNormalAccount.ErrorMessage);
        }

        private bool PushCCBySportToMainDB(IEnumerable<InsertNormalAccountCustInfoEntity> resultCustNAInserted, IEnumerable<GetNormalAccountBySportEntity> specialCCAccounts, int batchExternalSize)
        {
            var pushCCList = new List<InsertNormalAccountCustInfoEntity>();

            if (resultCustNAInserted.IsNotNullOrEmpty())
            {
                pushCCList.AddRange(resultCustNAInserted);
            }

            if (specialCCAccounts.IsNotNullOrEmpty())
            {
                pushCCList.AddRange(specialCCAccounts.Select(x => new InsertNormalAccountCustInfoEntity
                {
                    CustID = x.CustId,
                    SportGroup = x.SportGroup
                }));
            }

            return custClassToMainDBService.PushCCBySportToMainDB(pushCCList, this.serviceName, batchExternalSize);
        }

        private InsertNormalAccountBySportModel InsertNormalAccount(IEnumerable<GetNormalAccountBySportEntity> custs)
        {
            if (custs.IsNullOrEmpty())
            {
                return default;
            }

            var insertNA = custs.Select(x => new NormalAccountBySportEntity
            {
                CustId = x.CustId,
                CategoryId = x.CategoryId,
                TurnoverRM = x.TurnoverRM,
                WinlossRM = x.WinlossRM,
                BetCount = x.BetCount,
                ActiveDays = x.ActiveDays,
                SportGroup = x.SportGroup,
                PerformanceTime = x.PerformanceTime
            });

            return normalAccountInsertService.InsertNormalAccount(insertNA, this.serviceName);            
        }
    }
}
