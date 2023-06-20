namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface IDailyNormalClassificationJobService
    {
        Task<bool> Execute();
    }

    public class DailyNormalClassificationJobService : IDailyNormalClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IDailyNormalClassificationServices dailyClassificationServices;

        public DailyNormalClassificationJobService(
            ISentryClient sentryClient,
            IDailyNormalClassificationServices dailyClassificationServices)
        {
            this.sentryClient = sentryClient;
            this.dailyClassificationServices = dailyClassificationServices;
            this.serviceName = "DailyNormalClassification";
        }

        public Task<bool> Execute()
        {
            try
            {
                JobRun();
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        #region Private Methods

        private void JobRun()
        {
            bool isContinue;

            do
            {
                isContinue = dailyClassificationServices.Start()
                                                        .GetCustomerForDailyNormalScanning()
                                                        .ClassifyDailyNormal()
                                                        .UpdateLastScan();
            } while (isContinue);
        }
    }

    #endregion Private Methods
}