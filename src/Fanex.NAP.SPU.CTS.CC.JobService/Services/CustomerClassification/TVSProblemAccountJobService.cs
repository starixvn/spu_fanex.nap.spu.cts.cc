namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITVSProblemAccountJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class TVSProblemAccountJobService : ITVSProblemAccountJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly ITVSProblemAccountServices tvsProblemAccountServices;
        private readonly ISystemParameterServices systemParameterServices;
    

        public TVSProblemAccountJobService(
            ISentryClient sentryClient,
            IAppSettings appSettings,
            ITVSProblemAccountServices tvsProblemAccountServices,
            ISystemParameterServices systemParameterServices)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.tvsProblemAccountServices = tvsProblemAccountServices;
            this.systemParameterServices = systemParameterServices;
            this.serviceName = "TVSProblemAccountInsert";
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
                    Utilities.LogSentryError(serviceName, "InValid Batch size", batchSize, new Exception(), sentryClient);
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
            var isSuccess = true;

            var lastDateTime = systemParameterServices.GetLastScannedDateTime(SystemParameters.TVS_LastScanDateTime);

            var tvsProblemAccounts = tvsProblemAccountServices.GetTVSProblemAccounts(lastDateTime);

            if (tvsProblemAccounts != null && tvsProblemAccounts.ProblemAccounts.Any())
            {
                var paramaters = new RescanParametersEntity()
                {
                    InternalBatchSize = batchSize,
                    ExternalBatchSize = batchSize,
                    FeedSoon88API = appSettings.Soon88FeedDataAPI,
                    IsPushCCToSoon88 = true,
                    IsUpdateDangerToMainDB = true,
                    FunctionId = FunctionId.InsertNewCustJob,
                    ServiceName = serviceName,
                    IsWriteLog = false,
                    IsSyncAFCData = appSettings.IsSyncAFCData,
                    AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                    AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                };

                isSuccess = tvsProblemAccountServices.InsertTVSProblemAccounts(tvsProblemAccounts.ProblemAccounts, paramaters);
            }

            if (isSuccess && tvsProblemAccounts != null)
            {
                systemParameterServices.UpdateTimingExecution(tvsProblemAccounts.MaxTimeRequestActive?.ToString(CONVERT_TO_SQL_DATETIME), SystemParameters.TVS_LastScanDateTime);
            }
        }
    }

    #endregion Private Methods
}