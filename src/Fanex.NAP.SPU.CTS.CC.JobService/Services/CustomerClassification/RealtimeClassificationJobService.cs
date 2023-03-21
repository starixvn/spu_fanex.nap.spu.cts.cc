namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface IRealtimeClassificationJobService
    {
        Task<bool> ProcessClassification(int batchExternalSize, int batchInternalSize);

        Task<bool> CheckChanges();
    }

    public class RealtimeClassificationJobService : IRealtimeClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IRealtimeClassificationServices realtimeClassificationServices;

        public RealtimeClassificationJobService(
            ISentryClient sentryClient,
            IRealtimeClassificationServices realtimeClassificationServices)
        {
            this.sentryClient = sentryClient;
            this.realtimeClassificationServices = realtimeClassificationServices;
            this.serviceName = "RealtimeClassification";
        }

        public Task<bool> ProcessClassification(int batchExternalSize, int batchInternalSize)
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

        public Task<bool> CheckChanges()
        {
            try
            {
                realtimeClassificationServices.CheckNewChangesForRealtime();
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
                .GetNewPlaceBetCustomers()
                .GetCustomerCategory(batchInternalSize)
                .ClassifyRealtimePerformance(batchExternalSize)
                .UpdateLastTicketAssociationCustomer(batchInternalSize)
                .InsertPADailyQueue(batchInternalSize)
                .UpdateGeneralRealtimeLastScan();
        }
    }

    #endregion Private Methods
}