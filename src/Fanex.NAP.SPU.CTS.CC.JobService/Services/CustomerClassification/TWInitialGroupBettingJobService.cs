namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWInitialGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ITWInitialGroupBettingJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class TWInitialGroupBettingJobService : ITWInitialGroupBettingJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly ITWInitialGroupBettingService twInitialGBService;

        public TWInitialGroupBettingJobService(ISentryClient sentryClient, IAppSettings appSettings, ITWInitialGroupBettingService twInitialGBService)
        {
            this.sentryClient = sentryClient;
            this.twInitialGBService = twInitialGBService;        
            this.appSettings = appSettings;
            this.serviceName = "TWInitialGroupBettingJobService";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                JobRun(batchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchSize)
        {
            bool isContinue;

            var paras = new PushDataToExternalEntity
            {
                ExternalBatchSize = batchSize,
                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                IsPushCCToSoon88 = true,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                ServiceName = serviceName
            };

            do
            {
                var twInitialGB = twInitialGBService.GetChanges();

                isContinue = twInitialGB.CustIDs != null && twInitialGB.CustIDs.Any();

                if (isContinue)
                {
                    twInitialGBService.InsertTWInitalGBClassification(twInitialGB.CustIDs, paras);
                }

            } while (isContinue);

        }

    }
}
