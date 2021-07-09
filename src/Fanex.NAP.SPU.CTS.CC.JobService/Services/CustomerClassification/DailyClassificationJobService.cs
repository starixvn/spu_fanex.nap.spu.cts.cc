namespace Fanex.NAP.SPU.CTS.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.DailyClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDailyClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class DailyClassificationJobService : IDailyClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IDailyClassificationServices dailyClassificationServices;

        public DailyClassificationJobService(
            ISentryClient sentryClient,
            IDailyClassificationServices dailyClassificationServices)
        {
            this.sentryClient = sentryClient;
            this.dailyClassificationServices = dailyClassificationServices;
            this.serviceName = "DailyClassification";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                Utilities.LogSentryInfo(serviceName, $"{DateTime.Now}", string.Empty, true, sentryClient);

                if (batchSize > 0)
                {
                    JobRun(batchSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (batchSize), new NullReferenceException(), sentryClient);
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

        private void JobRun(int batchSize)
        {
            try
            {
                var customers = dailyClassificationServices.GetCustClassificationForDailyScanning(1, batchSize);

                if (customers != null && customers.Any())
                {
                    dailyClassificationServices.ScanningDailyClassification(customers, true);

                    JobRun(batchSize);
                }
            }
            catch(TimeoutException ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, (batchSize), ex, sentryClient);

                JobRun(batchSize);
            }
        }
    }

    #endregion Private Methods
}