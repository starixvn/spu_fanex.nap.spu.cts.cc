namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotAccountInsertJobService
    {
        Task<bool> ScanningRobotClassification(int batchExternalSize, int batchInternalSize);

        Task<bool> ScanningRobotAIClassification(int batchExternalSize, int batchInternalSize);
    }

    public class RobotAccountInsertJobService : IRobotAccountInsertJobService
    {
        private readonly string robotAccountServiceName;
        private readonly string robotAIClassServiceName;
        private readonly ISentryClient sentryClient;
        private readonly IRobotAccountInsertServices robotAccountInsertServices;
        private readonly IRobotAIClassificationInsertServices robotAIClassificationInsertServices;
        private readonly IAppSettings appSettings;

        public RobotAccountInsertJobService(
            ISentryClient sentryClient,
            IRobotAccountInsertServices robotAccountInsertServices,
            IRobotAIClassificationInsertServices robotAIClassificationInsertServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.robotAccountInsertServices = robotAccountInsertServices;
            this.robotAIClassificationInsertServices = robotAIClassificationInsertServices;
            this.appSettings = appSettings;
            this.robotAccountServiceName = "RobotAccountInsert";
            this.robotAIClassServiceName = "RobotAIClassification";
        }

        public Task<bool> ScanningRobotClassification(int batchExternalSize, int batchInternalSize)
        {
            var response = true;

            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    response = InsertRobotCategoryFromMainDB(batchExternalSize, batchInternalSize);
                }
                else
                {
                    Utilities.LogSentryError(robotAccountServiceName, "InValid Batch size", (batchExternalSize, batchInternalSize), new Exception(), sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(robotAccountServiceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(response);
        }   
        
        public Task<bool> ScanningRobotAIClassification(int batchExternalSize, int batchInternalSize)
        {
            var response = true;

            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    response = InsertRobotCategoryFromAIClassification(batchExternalSize, batchInternalSize);
                }
                else
                {
                    Utilities.LogSentryError(robotAIClassServiceName, "InValid Batch size", (batchExternalSize, batchInternalSize), new Exception(), sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(robotAIClassServiceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(response);
        }

        #region Private Methods

        private bool InsertRobotCategoryFromMainDB(int batchExternalSize, int batchInternalSize)
        {
            var isSuccess = robotAccountInsertServices.Start()
                                                      .GetLastTimespan()
                                                      .GetRobotUser()
                                                      .Process(new InsertRobotParaEntity
                                                      {
                                                          BatchExternalSize = batchExternalSize,
                                                          BatchInternalSize = batchInternalSize,
                                                          ServiceName = robotAccountServiceName,
                                                          Soon88FeedDataAPI = appSettings.Soon88FeedDataAPI,
                                                          IsWriteLog = appSettings.RobotAccountIsWriteLog,
                                                          IsSyncAFCData = appSettings.IsSyncAFCData,
                                                          AFCInsertAccountURL = appSettings.AFCInsertAccountURL
                                                      });


            if (isSuccess)
            {
                robotAccountInsertServices.UpdateLastTimespan();
            }

            return isSuccess;
        }

        private bool InsertRobotCategoryFromAIClassification(int batchExternalSize, int batchInternalSize)
        {
            var isSuccess = robotAIClassificationInsertServices.Start()
                                                               .GetRobotUserFromAIClassification()
                                                               .Process(new InsertRobotParaEntity
                                                               {
                                                                  BatchExternalSize = batchExternalSize,
                                                                  BatchInternalSize = batchInternalSize,
                                                                  ServiceName = robotAIClassServiceName,
                                                                  Soon88FeedDataAPI = appSettings.Soon88FeedDataAPI,
                                                                  IsWriteLog = appSettings.RobotAccountIsWriteLog
                                                               });


            if (isSuccess)
            {
                robotAIClassificationInsertServices.UpdateLastScannedId(SystemParameters.RobotAIClassificationLastScannedID);
            }

            return isSuccess;
        }
    }

    #endregion Private Methods
}