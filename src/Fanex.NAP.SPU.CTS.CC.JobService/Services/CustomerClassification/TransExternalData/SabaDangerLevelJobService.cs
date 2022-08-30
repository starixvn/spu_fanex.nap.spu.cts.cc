namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface ISabaDangerLevelJobService
    {
        Task<bool> Execute(int batchSize);

        Task<bool> InitializeData(int batchSize);
    }

    public class SabaDangerLevelJobService : ISabaDangerLevelJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly ISabaDangerLevelServices sabaDangerLevelServices;

        public SabaDangerLevelJobService(
            ISentryClient sentryClient,
            ISabaDangerLevelServices sabaDangerLevelServices)
        {
            this.sentryClient = sentryClient;
            this.sabaDangerLevelServices = sabaDangerLevelServices;
            this.serviceName = "SabaDangerLevelJobService";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                return Task.FromResult(JobRun(batchSize));
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }
        }

        public Task<bool> InitializeData(int batchSize)
        {
            try
            {
                return Task.FromResult(JobRunForInitializing(batchSize));
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }
        }

        #region Private Methods

        private bool JobRun(int batchSize)
        {
            var isContinue = sabaDangerLevelServices
                                             .Start()
                                             .GetSabaDangerLevelForScanning();

            var isSuccess = sabaDangerLevelServices
                                             .UpdateSabaDangerLevelToMainDB(sentryClient, serviceName, batchSize)
                                             .UpdateLogSabaDangerLevel()
                                             .UpdateLastScan();
            if (isContinue && isSuccess)
            {
                JobRun(batchSize);
            }

            return isSuccess;
        }

        private bool JobRunForInitializing(int batchSize)
        {
            var isContinue = sabaDangerLevelServices.StartForInitialzingData()
                                                    .GetSabaDangerLevelForInitialization();

            var isSuccess = sabaDangerLevelServices.UpdateSabaDangerLevelToMainDBForInitialization(sentryClient, serviceName, batchSize)
                                                   .UpdateLogSabaDangerLevelForInitialization()
                                                   .UpdateLastScanForInitialization();

            if (isContinue && isSuccess)
            {
                JobRunForInitializing(batchSize);
            }

            return isSuccess;
        }
    }

    #endregion Private Methods
}