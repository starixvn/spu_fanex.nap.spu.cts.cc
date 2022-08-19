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
                return Task.FromResult(JobRun());
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }
        }

        public Task<bool> InitializeData()
        {
            try
            {
                return Task.FromResult(JobRunForInitializing());
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }
        }

        #region Private Methods

        private bool JobRun()
        {
            return sabaDangerLevelServices
                     .Start()
                     .GetSabaDangerLevelForScanning()
                     .UpdateSabaDangerLevelToMainDB()
                     .UpdateLogSabaDangerLevel()
                     .UpdateLastScan();
        }

        private bool JobRunForInitializing()
        {
            var isContinue = sabaDangerLevelServices.StartForInitialzingData()
                                                    .GetSabaDangerLevelForInitialization();

            var isSuccess = sabaDangerLevelServices.UpdateSabaDangerLevelToMainDBForInitialization()
                                                   .UpdateLogSabaDangerLevelForInitialization()
                                                   .UpdateLastScanForInitialization();

            if (!isSuccess)
            {
                return false;
            }    

            if (isContinue)
            {
                JobRunForInitializing();
            }

            return true;
        }
    }

    #endregion Private Methods
}