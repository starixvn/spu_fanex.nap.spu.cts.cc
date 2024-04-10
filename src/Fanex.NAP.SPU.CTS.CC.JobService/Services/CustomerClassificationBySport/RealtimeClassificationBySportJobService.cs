namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface IRealtimeClassificationBySportJobService
    {
        Task<bool> Execute(int batchInternalSize);
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
            this.serviceName = "Realtime Classification By Sport";
        }

        public Task<bool> Execute(int batchInternalSize)
        {
            try
            {                
                if (batchInternalSize <= 0)
                    batchInternalSize = 5000;

                JobRun(batchInternalSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchInternalSize)
        {
            this.realtimeClassificationServices.Start()
                .GetNewPlaceBetCustomers()
                .PreprocessCustomerClassRealtime(batchInternalSize)
                .CompleteCustomerRealtime();
        }
    }
}
