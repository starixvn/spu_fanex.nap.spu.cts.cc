namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
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

            if (newClassifications.NormalAccount != null && newClassifications.NormalAccount.Any())
            {
                var newClassificationChunks = newClassifications.NormalAccount.Chunk(batchInternalSize).ToList();

                foreach (var classifications in newClassificationChunks)
                {
                    var insertNA = classifications.Select(x => new NormalAccountBySportEntity
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

                    var insertNAResult = normalAccountInsertService.InsertNormalAccount(insertNA, this.serviceName);

                    if (string.IsNullOrEmpty(insertNAResult.ErrorMessage))
                    {
                        var isPushCCSuccess = custClassToMainDBService.PushCCBySportToMainDB(insertNAResult.CustNAInserted, this.serviceName, batchExternalSize);

                        var customerXML = insertNAResult.CustNAInserted.Select(x => new CustSportXMLModel {
                            CustID = x.CustID,
                            SportGroup = x.SportGroup
                        }).ToXmlString();

                        normalAccountInsertService.CompleteNormalAccountBySport(customerXML, newClassifications.ScannedMaxId, isPushCCSuccess);
                    }
                }

                isContinue = true;
            }

            return isContinue;
        }
    }
}
