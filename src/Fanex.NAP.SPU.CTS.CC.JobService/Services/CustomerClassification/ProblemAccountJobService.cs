namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);

        Task<bool> UpdateWriteLogSetting(bool isWriteLog);
    }

    public class ProblemAccountJobService : IProblemAccountJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAppSettings appSettings;
        private IEnumerable<ProblemAccountsEntity> logData;
        private readonly bool isWriteLog;

        public ProblemAccountJobService(
            ISentryClient sentryClient,
            IProblemAccountServices problemAccountServices,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService pushDataToExternalService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.problemAccountServices = problemAccountServices;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = pushDataToExternalService;
            this.appSettings = appSettings;
            this.isWriteLog = appSettings.ProblemAccountIsWriteLog;
            this.serviceName = "ProblemAccountInsert";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
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
                Utilities.LogSentryError(serviceName, ex.Message, logData, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> UpdateWriteLogSetting(bool isWriteLog)
        {
            try
            {
                bool result = appSettings.UpdateIsWriteLogSetting(serviceName, isWriteLog);

                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                sentryClient.CaptureException(e);
                return Task.FromResult(false);
            }
        }

        #region Private Methods

        private void JobRun(int batchExternalSize, int batchInternalSize)
        {
            var lastScannedTime = systemParameterServices.GetLastScannedDateTime(SystemParameters.ProblemAccountLastScannedTime);

            var savedDataTime = DateTime.Now;

            var problemAccounts = problemAccountServices.GetProblemAccounts(lastScannedTime, serviceName, isWriteLog);

            if (problemAccounts != null && problemAccounts.Any())
            {
                logData = problemAccounts;
                var problemAccountsChunk = problemAccounts.Distinct().Chunk(batchInternalSize);

                foreach (var items in problemAccountsChunk)
                {
                    var custIds = problemAccountServices.InsertProblemAccountToCTS(items, false, serviceName, isWriteLog);

                    if (custIds != null && custIds.Any())
                    {
                        custClassToMainDBService.PushDataToExternal(custIds, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = batchExternalSize,
                            FeedSoon88API = appSettings.Soon88FeedDataAPI,
                            IsUpdateDangerToMainDB = false,
                            IsPushCCToSoon88 = true,
                            IsWriteLog = isWriteLog,
                            ServiceName = serviceName
                        });
                    }
                }
            }

            systemParameterServices.UpdateTimingExecution(savedDataTime.ToString(Constants.CONVERT_TO_SQL_DATETIME), SystemParameters.ProblemAccountLastScannedTime);
        }

        #endregion Private Methods
    }
}