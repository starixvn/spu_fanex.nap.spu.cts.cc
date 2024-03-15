namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousScoreClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDailyDangerousScoreClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class DailyDangerousClassificationJobService : IDailyDangerousScoreClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IDailyDangerousClassificationService dailyDangerousClassificationService;
        private readonly ISystemParameterServices systemParameterService;
        private readonly IAppSettings appSettings;

        public DailyDangerousClassificationJobService(ISentryClient sentryClient,
            IDailyDangerousClassificationService dailyDangerousClassificationService,
            ISystemParameterServices systemParameterService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.dailyDangerousClassificationService = dailyDangerousClassificationService;
            this.appSettings = appSettings;
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
            var scannedDate = DateTime.Now;

            var dangerousCusts = dailyDangerousClassificationService.GetCustDangerousDetection(scannedDate);

            if (dangerousCusts != null && dangerousCusts.Any())
            {

            }
        }

    }
}
