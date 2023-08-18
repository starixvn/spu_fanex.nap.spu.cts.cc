namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate
{
    using System;
    using System.Threading.Tasks;
    using Sentry;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification;
    using Fanex.NAP.SPU.CTS.Shares;

    public interface INormalTaggingClassificationJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class NormalTaggingClassificationJobService : INormalTaggingClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient _sentryClient;
        private readonly INormalTaggingClassificationServices _normalTaggingClassificationServices;

        public NormalTaggingClassificationJobService(INormalTaggingClassificationServices normalTaggingClassificationServices, ISentryClient sentryClient)
        {
            _normalTaggingClassificationServices = normalTaggingClassificationServices;
            _sentryClient = sentryClient;
            this.serviceName = "NormalTaggingClassification";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                if (batchExternalSize >= batchInternalSize && batchInternalSize > 0)
                {
                    JobRun(batchExternalSize, batchInternalSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (batchExternalSize, batchInternalSize), new NullReferenceException(), _sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchExternalSize, int batchInternalSize)
        {
            bool isContinue;

            do
            {
                isContinue = _normalTaggingClassificationServices
                                .Start()
                                .GetNormalTaggingScanning(batchExternalSize)
                                .PreprocessCustomerClassNormalTagging(batchInternalSize)
                                .UpdateLastScanningNormalTaggingParameter();
            } while (isContinue);
        }
    }
}
