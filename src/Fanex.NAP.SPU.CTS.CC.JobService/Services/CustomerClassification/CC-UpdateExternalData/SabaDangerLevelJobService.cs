namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface ISabaDangerLevelJobService
    {
        Task<bool> Execute();

        Task<bool> InitializeData();
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

        public Task<bool> InitializeData()
        {
            try
            {
                JobRunForInitializing();
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
            sabaDangerLevelServices
                .Start()
                .GetSabaDangerLevelForScanning()
                .UpdateLogSabaDangerLevel()
                .UpdateLastScan();
        }

        private void JobRunForInitializing()
        {
            sabaDangerLevelServices
                .StartForInitialzingData()
                .GetSabaDangerLevelForInitializing()
                .UpdateLogSabaDangerLevelForInitializing()
                .UpdateLastScanForInitializing();
        }
    }

    #endregion Private Methods
}