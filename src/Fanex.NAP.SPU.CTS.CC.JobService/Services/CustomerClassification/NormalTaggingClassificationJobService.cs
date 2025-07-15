namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalTaggingClassification;
    using Fanex.NAP.SPU.CTS.Shares;

    public interface INormalTaggingClassificationJobService
    {
        Task<bool> Execute(int batchInternalSize);
    }

    public class NormalTaggingClassificationJobService : INormalTaggingClassificationJobService
    {
        private readonly string serviceName;
        private readonly INormalTaggingClassificationServices _normalTaggingClassificationServices;

        public NormalTaggingClassificationJobService(INormalTaggingClassificationServices normalTaggingClassificationServices)
        {
            _normalTaggingClassificationServices = normalTaggingClassificationServices;
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
                    Utilities.LogSentryError(serviceName, "InValid Batch size", batchInternalSize, new NullReferenceException());
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex);

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
