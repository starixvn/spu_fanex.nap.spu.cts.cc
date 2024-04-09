namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using System;
    using System.Threading.Tasks;
    using Sentry;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalTaggingClassification;
    using Fanex.NAP.SPU.CTS.Shares;

    public interface INormalTaggingClassificationJobService
    {
        Task<bool> Execute(int batchInternalSize);
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

        public Task<bool> Execute(int batchInternalSize)
        {
            try
            {
                if (batchInternalSize > 0)
                {
                    JobRun(batchInternalSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", batchInternalSize, new NullReferenceException(), _sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchInternalSize)
        {
            bool isContinue;

            do
            {
                isContinue = _normalTaggingClassificationServices
                                .Start()
                                .GetNormalTaggingScanning()
                                .PreprocessCustomerClassNormalTagging(batchInternalSize)
                                .UpdateLastScanningNormalTaggingParameter();
            } while (isContinue);
        }
    }
}
