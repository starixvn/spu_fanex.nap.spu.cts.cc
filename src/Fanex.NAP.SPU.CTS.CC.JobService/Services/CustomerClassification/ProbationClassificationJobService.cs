namespace Fanex.NAP.SPU.CTS.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProbationClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface IProbationClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class ProbationClassificationJobService : IProbationClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IProbationClassificationServices probationClassificationServices;

        public ProbationClassificationJobService(
            ISentryClient sentryClient,
            IProbationClassificationServices probationClassificationServices)
        {
            this.sentryClient = sentryClient;
            this.probationClassificationServices = probationClassificationServices;
            this.serviceName = "ProbationClassification";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
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
            probationClassificationServices
                .GetProbationCustForScanning(batchSize)
                .ScanningProbationClassification()
                .UpdateLastScanDateForProbationCust();
        }
    }

    #endregion Private Methods
}