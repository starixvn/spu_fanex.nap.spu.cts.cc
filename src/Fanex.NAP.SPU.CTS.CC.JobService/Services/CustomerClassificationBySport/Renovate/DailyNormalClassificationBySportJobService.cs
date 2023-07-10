namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDailyNormalClassificationBySportJobService
    {
        Task<bool> Execute();
    }

    public class DailyNormalClassificationBySportJobService : IDailyNormalClassificationBySportJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IDailyNormalClassificationBySportServices dailyService;

        public DailyNormalClassificationBySportJobService(ISentryClient sentryClient,
            IDailyNormalClassificationBySportServices dailyService)
        {
            this.sentryClient = sentryClient;
            this.dailyService = dailyService;
            this.serviceName = "DailyNormalClassificationBySportRenovate";
        }

        public Task<bool> Execute()
        {
            try
            {
                var isContinue = true;

                do
                {
                    isContinue = JobRun();

                } while (isContinue);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool JobRun()
        {
            return this.dailyService.Start()
                .GetCustClassificationForDailyScanning()
                .PreprocessDailyNormalClassification()
                .UpdateLastScan();
        }
    }
}
