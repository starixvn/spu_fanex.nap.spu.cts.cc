namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.RealtimeClassificationBySport;
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

        public RealtimeClassificationBySportJobService(ISentryClient sentryClient,
            IRealtimeClassificationBySportServices realtimeClassificationServices)
        {
            this.sentryClient = sentryClient;
            this.realtimeClassificationServices = realtimeClassificationServices;
            this.serviceName = "Realtime Classification By Sport Renovate";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                if (batchExternalSize <= 0)
                    batchExternalSize = 3000;

                if (batchInternalSize <= 0)
                    batchInternalSize = 1000;

                JobRun(batchExternalSize, batchInternalSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchExternalSize, int batchInternalSize)
        {
            this.realtimeClassificationServices.Start()
                .GetNewPlaceBetCustomers()
                .PreprocessCustomerClassRealtime(batchExternalSize, batchInternalSize)
                .CompleteCustomerRealtime();
        }
    }
}
