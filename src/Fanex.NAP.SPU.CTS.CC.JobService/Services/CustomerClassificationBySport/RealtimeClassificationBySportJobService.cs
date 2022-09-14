namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface IRealtimeClassificationBySportJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class RealtimeClassificationBySportJobService : IRealtimeClassificationBySportJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IRealtimeClassificationBySportServices realtimeClassificationServices;

        public RealtimeClassificationBySportJobService(
            ISentryClient sentryClient,
            IRealtimeClassificationBySportServices realtimeClassificationServices)
        {
            this.sentryClient = sentryClient;
            this.realtimeClassificationServices = realtimeClassificationServices;
            this.serviceName = "RealtimeClassificationBySport";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    JobRun(batchExternalSize, batchInternalSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (batchExternalSize, batchInternalSize), new NullReferenceException(), sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        #region Private Methods

        private void JobRun(int batchExternalSize, int batchInternalSize)
        {
            realtimeClassificationServices
                .Start()
                .GetCustomerGeneralCategoryHasChangeForRunRealtime()
                .ScanningRealtimeClassification(batchExternalSize, batchInternalSize)
                .RealtimeUpdateLastScanTime(true);
        }
    }

    #endregion Private Methods
}