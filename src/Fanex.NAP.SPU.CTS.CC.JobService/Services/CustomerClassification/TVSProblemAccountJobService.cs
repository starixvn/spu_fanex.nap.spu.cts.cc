namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
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
        private readonly IDataLogTrackingServices dataLogTrackingServices;
        private readonly IIncorrectDataMonitoringService monitoringService;        

        public TVSProblemAccountJobService(
            ISentryClient sentryClient,
            IAppSettings appSettings,
            ITVSProblemAccountServices tvsProblemAccountServices,
            ISystemParameterServices systemParameterServices,
            IDataLogTrackingServices dataLogTrackingServices,
            IIncorrectDataMonitoringService monitoringService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.tvsProblemAccountServices = tvsProblemAccountServices;
            this.systemParameterServices = systemParameterServices;
            this.dataLogTrackingServices = dataLogTrackingServices;
            this.monitoringService = monitoringService;
            this.serviceName = "TVSProblemAccountInsert";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                if (batchSize > 0)
                {
                    return Task.FromResult(JobRun(batchSize));
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", batchSize, new Exception(), sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                dataLogTrackingServices.LogErrorData(ex, serviceName);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        #region Private Methods

        private bool JobRun(int batchSize)
        {
            var isSuccess = true;

            var lastDateTime = systemParameterServices.GetLastScannedDateTime(CTS_SystemParameters.TVS_LastScanDateTime);

            var tvsProblemAccounts = tvsProblemAccountServices.GetTVSProblemAccounts(lastDateTime);

            if (tvsProblemAccounts.ProblemAccounts.Any())
            {
                var paramaters = new PushDataToExternalEntity()
                {
                    IsPushCCToSoon88 = true,
                    FeedSoon88API = appSettings.Soon88FeedDataAPI,
                    IsUpdateDangerToMainDB = true,
                    ExternalBatchSize = batchSize,
                    ServiceName = serviceName,
                    IsSyncAFCData = appSettings.IsSyncAFCData,
                    AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                    AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                };

                isSuccess = tvsProblemAccountServices.InsertTVSProblemAccounts(tvsProblemAccounts.ProblemAccounts, paramaters);

                if (isSuccess)
                {
                    paramaters.IsSyncAFCData = false;
                    isSuccess = tvsProblemAccountServices.InsertTVSRobot(tvsProblemAccounts.ProblemAccounts, paramaters);
                }
            }

            if (isSuccess)
            {
                systemParameterServices.UpdateTimingExecution(tvsProblemAccounts.MaxTimeRequestActive?.ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.TVS_LastScanDateTime);
            }

            if (!string.IsNullOrEmpty(tvsProblemAccounts.MissingDataRequestIds))
            {
                Utilities.LogSentryError(serviceName, "Missing TVS RequestId(s): ", tvsProblemAccounts.MissingDataRequestIds, new Exception(), sentryClient);

                InsertIncorrectDataMonitoring(tvsProblemAccounts.MissingDataRequestIds);
            } 

            return isSuccess;
        }

        private void InsertIncorrectDataMonitoring(string missingData)
        {
            if (!string.IsNullOrEmpty(missingData))
            {
                var dataToList = missingData.Split(",").ToList();

                var dataChunks = dataToList.Chunk(50);

                foreach (var logDataBatch in dataChunks)
                {
                    var dataToString = string.Join(",", logDataBatch);
                    monitoringService.InsertIncorrectDataMonitoring(dataToString, null, JobMonitor_IssueType.Classification_MissingTVSProblemAccount);
                }
            }
        }
    }

    #endregion Private Methods
}