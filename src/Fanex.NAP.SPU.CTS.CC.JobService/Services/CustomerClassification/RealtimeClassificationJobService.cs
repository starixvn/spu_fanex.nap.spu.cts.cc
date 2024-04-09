namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRealtimeClassificationJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class RealtimeClassificationJobService : IRealtimeClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IRealtimeClassificationService realtimeServices;

        public RealtimeClassificationJobService(ISentryClient sentryClient, IRealtimeClassificationService realtimeServices)
        {
            this.sentryClient = sentryClient;
            this.realtimeServices = realtimeServices;
            this.serviceName = "RealtimeClassification";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    this.realtimeServices.Start()
                        .GetNewPlaceBetCustomers()
                        .GetCustomerCategory(batchInternalSize);

                    Parallel.Invoke(() =>
                    {
                        this.realtimeServices.PreprocessCustomerClassRealtime(batchInternalSize);
                    },
                    () =>
                    {
                        this.realtimeServices.InsertPADailyScanQueue(batchInternalSize);
                    });

                    this.realtimeServices.CompleteCustomerRealtime();
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
    }
}
