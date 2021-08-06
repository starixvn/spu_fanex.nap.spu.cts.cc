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
            this.serviceName = "ProblemAccountInsert";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            bool response = true;
            
            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    response = JobRun(batchExternalSize, batchInternalSize);
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

            return Task.FromResult(response);
        }

        #region Private Methods

        private bool JobRun(int batchExternalSize, int batchInternalSize)
        {
            var lastScannedTime = systemParameterServices.GetLastScannedDateTime(SystemParameters.ProblemAccountLastScannedTime);

            var savedDataTime = DateTime.Now;

            var problemAccounts = problemAccountServices.GetProblemAccounts(lastScannedTime, serviceName, appSettings.ProblemAccountIsWriteLog);

            var isSuccess = InsertProblemAccount(problemAccounts, batchExternalSize, batchInternalSize);

            if (isSuccess)
            {
                systemParameterServices.UpdateTimingExecution(savedDataTime.ToString(Constants.CONVERT_TO_SQL_DATETIME), SystemParameters.ProblemAccountLastScannedTime);
            }

            return isSuccess;
        }

        private bool InsertProblemAccount(IEnumerable<ProblemAccountsEntity> problemAccounts, int batchExternalSize, int batchInternalSize)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                logData = problemAccounts;
                custClassToMainDBService.LogInputData(new LogInputViewEntity { ProblemAccounts = problemAccounts }, FunctionId.InsertPAJob, serviceName);

                var problemAccountsChunk = problemAccounts.Distinct().Chunk(batchInternalSize);

                foreach (var items in problemAccountsChunk)
                {
                    var result = problemAccountServices.InsertProblemAccountToCTS(items, false, serviceName, appSettings.ProblemAccountIsWriteLog);
                    var isSuccess = (result != null && result.ErrorCode == 0);

                    if (result != null && result.CustIds != null && result.CustIds.Any())
                    {
                        isSuccess = isSuccess && custClassToMainDBService.PushDataToExternal(result.CustIds, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = batchExternalSize,
                            FeedSoon88API = appSettings.Soon88FeedDataAPI,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = false,
                            IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                            ServiceName = serviceName
                        });
                    }

                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion Private Methods
    }
}