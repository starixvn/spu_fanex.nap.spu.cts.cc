namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NewTWGroupBettingClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface ITWGBTaggingClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class TWGBTaggingClassificationJobService : ITWGBTaggingClassificationJobService
    {
        private readonly string _serviceName;
        private readonly ISentryClient _sentryClient;
        private readonly ITWGBTaggingClassificationService _service;

        public TWGBTaggingClassificationJobService(ISentryClient sentryClient, ITWGBTaggingClassificationService service)
        {
            _serviceName = "TWGBTaggingClassification";
            _sentryClient = sentryClient;
            _service = service;
        }

        public Task<bool> Execute(int batchSize)
        {
            if (batchSize <= 0)
            {
                batchSize = 4000;
            }

            try
            {
                JobRun(batchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(_serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchSize)
        {
            _service.GetChanges()
                    .UpdateNextScannedTime()
                    .PreprocessWithBatch(batchSize);
        }
    }
}
