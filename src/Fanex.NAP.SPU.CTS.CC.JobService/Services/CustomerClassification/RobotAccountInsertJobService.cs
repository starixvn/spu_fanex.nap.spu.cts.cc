namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotAccountInsertJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class RobotAccountInsertJobService : IRobotAccountInsertJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IRobotAccountInsertServices robotAccountInsertServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IAppSettings appSettings;

        public RobotAccountInsertJobService(
            ISentryClient sentryClient,
            IRobotAccountInsertServices robotAccountInsertServices,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IProblemAccountServices problemAccountServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.robotAccountInsertServices = robotAccountInsertServices;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountServices = problemAccountServices;
            this.appSettings = appSettings;
            this.serviceName = "RobotAccountInsert";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            var response = true;

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
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(response);
        }

        #region Private Methods

        private bool JobRun(int batchExternalSize, int batchInternalSize)
        {
            var lastScannedTime = systemParameterServices.GetLastScannedDateTime(SystemParameters.RobotAccountLastScannedTime);

            var savedExecutionTime = DateTime.Now.ToString(Constants.CONVERT_TO_SQL_DATETIME);

            var robotUsers = robotAccountInsertServices.GetRobotUser(lastScannedTime, appSettings.RobotAccountCreditSites);

            var isSuccess = InsertRobotUsers(robotUsers, batchExternalSize, batchInternalSize);

            if (isSuccess)
            {
                systemParameterServices.UpdateTimingExecution(savedExecutionTime, SystemParameters.RobotAccountLastScannedTime);
            }

            return isSuccess;
        }

        private bool InsertRobotUsers(IEnumerable<RobotUserEntity> robotUsers, int batchExternalSize, int batchInternalSize)
        {
            if (robotUsers != null && robotUsers.Any())
            {
                custClassToMainDBService.LogInputData(new LogInputViewEntity { RobotUser = robotUsers }, FunctionId.RobotUserJob, serviceName);

                var robotUsersChunks = robotUsers.Chunk(batchInternalSize).ToList();

                foreach (var robot in robotUsersChunks)
                {
                    var custCategory = robotAccountInsertServices.MappingRobotCategory(robot);
                    var result = problemAccountServices.InsertProblemAccountToCTS(custCategory, false, serviceName, appSettings.RobotAccountIsWriteLog);
                    var isSuccess = (result != null && result.ErrorCode == 0);

                    if (result != null && result.CustIds != null && result.CustIds.Any())
                    {
                        isSuccess = isSuccess && custClassToMainDBService.PushDataToExternal(result.CustIds, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = batchExternalSize,
                            FeedSoon88API = appSettings.Soon88FeedDataAPI,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = false,
                            IsWriteLog = appSettings.RobotAccountIsWriteLog,
                            ServiceName = serviceName
                        });

                        Thread.Sleep(RobotUsers_SleepTimes);
                    }

                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    #endregion Private Methods
}