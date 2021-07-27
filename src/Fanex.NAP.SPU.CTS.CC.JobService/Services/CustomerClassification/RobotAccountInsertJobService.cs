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
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        #region Private Methods

        private void JobRun(int externalBatchSize, int internalBatchSize)
        {
            var lastScannedTime = systemParameterServices.GetLastScannedDateTime(SystemParameters.RobotAccountLastScannedTime);

            var savedExecutionTime = DateTime.Now.ToString(Constants.CONVERT_TO_SQL_DATETIME);

            var robotUsers = robotAccountInsertServices.GetRobotUser(lastScannedTime, appSettings.RobotAccountCreditSites);
            
            custClassToMainDBService.LogInputData(new LogInputViewEntity { RobotUser = robotUsers }, FunctionId.RobotUserJob, serviceName);

            if (robotUsers != null && robotUsers.Any())
            {
                var robotUsersChunks = robotUsers.Chunk(internalBatchSize).ToList();

                foreach (var robot in robotUsersChunks)
                {
                    var custCategory = robotAccountInsertServices.MappingRobotCategory(robot);

                    var custIds = problemAccountServices.InsertProblemAccountToCTS(custCategory, false, serviceName, appSettings.RobotAccountIsWriteLog);

                    if (custIds != null && custIds.Any())
                    {
                        custClassToMainDBService.PushDataToExternal(custIds, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = externalBatchSize,
                            FeedSoon88API = appSettings.Soon88FeedDataAPI,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = false,
                            IsWriteLog = appSettings.RobotAccountIsWriteLog,
                            ServiceName = serviceName
                        });
                    }
                }
            }

            systemParameterServices.UpdateTimingExecution(savedExecutionTime, SystemParameters.RobotAccountLastScannedTime);
        }
    }

    #endregion Private Methods
}