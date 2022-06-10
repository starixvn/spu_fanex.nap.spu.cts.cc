namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotAccountInsertJobService
    {
        Task<bool> ScanningRobotClassification(int batchExternalSize, int batchInternalSize);

        Task<bool> ScanningRobotAIClassification(int batchExternalSize, int batchInternalSize);

        Task<bool> ScanningRobotDetection(int batchExternalSize, int batchInternalSize);
    }

    public class RobotAccountInsertJobService : IRobotAccountInsertJobService
    {
        private readonly string robotAccountServiceName;
        private readonly string robotAIClassServiceName;
        private readonly string robotDetectionServiceName;
        private readonly ISentryClient sentryClient;
        private readonly IRobotAccountInsertServices robotAccountInsertServices;
        private readonly IRobotAIClassificationInsertServices robotAIClassificationInsertServices;
        private readonly IRobotDetectionService robotDetectionService;
        private readonly IAppSettings appSettings;

        public RobotAccountInsertJobService(
            ISentryClient sentryClient,
            IRobotAccountInsertServices robotAccountInsertServices,
            IRobotAIClassificationInsertServices robotAIClassificationInsertServices,
            IRobotDetectionService robotDetectionService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.robotAccountInsertServices = robotAccountInsertServices;
            this.robotAIClassificationInsertServices = robotAIClassificationInsertServices;
            this.robotDetectionService = robotDetectionService;
            this.appSettings = appSettings;
            this.robotAccountServiceName = "RobotAccountInsert";
            this.robotAIClassServiceName = "RobotAIClassification";
            this.robotDetectionServiceName = "RobotDetection";
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

        public Task<bool> ScanningRobotDetection(int batchExternalSize, int batchInternalSize)
        {
            var response = true;

            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    response = InsertRobotDetection(batchExternalSize, batchInternalSize);
                }
                else
                {
                    Utilities.LogSentryError(robotDetectionServiceName, "InValid Batch size", (batchExternalSize, batchInternalSize), new Exception(), sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(robotDetectionServiceName, ex.Message, string.Empty, ex, sentryClient);

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
                                                          BatchInternalSize = batchInternalSize,
                                                          IsFromTW = true,
                                                          IsFromCTS = false,                                                         
                                                          IsDirect = true,
                                                          IsFromAI = false,
                                                          IsFromTVS = false,
                                                          PushDataExternal = new PushDataToExternalEntity
                                                          {
                                                              IsUpdateDangerToMainDB = true,
                                                              IsPushCCToSoon88 = true,
                                                              FeedSoon88API = appSettings.Soon88FeedDataAPI,
                                                              ServiceName = robotAccountServiceName,
                                                              ExternalBatchSize = batchExternalSize,
                                                              IsWriteLog = appSettings.RobotAccountIsWriteLog,
                                                              IsSyncAFCData = false,
                                                              AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                                                              AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                                                          }
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
                                                                   BatchInternalSize = batchInternalSize,
                                                                   IsFromTW = false,
                                                                   IsFromCTS = false,
                                                                   IsDirect = true,
                                                                   IsFromAI = false,
                                                                   IsFromTVS = false,
                                                                   PushDataExternal = new PushDataToExternalEntity
                                                                   {
                                                                       IsUpdateDangerToMainDB = true,
                                                                       IsPushCCToSoon88 = true,
                                                                       FeedSoon88API = appSettings.Soon88FeedDataAPI,
                                                                       ServiceName = robotAIClassServiceName,
                                                                       ExternalBatchSize = batchExternalSize,
                                                                       IsWriteLog = appSettings.RobotAccountIsWriteLog,
                                                                       IsSyncAFCData = false,
                                                                       AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                                                                       AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                                                                   }
                                                               });

            if (isSuccess)
            {
                robotAIClassificationInsertServices.UpdateLastScannedId(SystemParameters.RobotAIClassificationLastScannedID);
            }

            return isSuccess;
        }

        public bool InsertRobotDetection(int batchExternalSize, int batchInternalSize)
        {
            var isSuccess = robotDetectionService.Start()
                                                 .GetRobotDetections(SystemParameters.RobotDetectionLastModifiedDate)
                                                 .Process(new InsertRobotParaEntity
                                                 {
                                                     BatchInternalSize = batchInternalSize,
                                                     IsFromTW = false,
                                                     IsFromCTS = false,
                                                     IsDirect = true,
                                                     IsFromAI = true,
                                                     IsFromTVS = false,
                                                     PushDataExternal = new PushDataToExternalEntity
                                                     {
                                                         IsUpdateDangerToMainDB = true,
                                                         IsPushCCToSoon88 = true,
                                                         FeedSoon88API = appSettings.Soon88FeedDataAPI,
                                                         ServiceName = robotDetectionServiceName,
                                                         ExternalBatchSize = batchExternalSize,
                                                         IsWriteLog = appSettings.RobotAccountIsWriteLog,
                                                         IsSyncAFCData = false,
                                                         AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                                                         AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                                                     }
                                                 });

            if (isSuccess)
            {
                robotDetectionService.UpdateLastScannedId(SystemParameters.RobotDetectionLastModifiedDate);
            }

            return isSuccess;
        }
    }

    #endregion Private Methods
}