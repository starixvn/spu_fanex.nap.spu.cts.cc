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
        Task<bool> ScanningTWRobotAccount(int batchExternalSize, int batchInternalSize);

        Task<bool> ScanningAIRobotClassification(int batchExternalSize, int batchInternalSize);
    }

    public class RobotAccountInsertJobService : IRobotAccountInsertJobService
    {
        private readonly string robotAccountServiceName;
        private readonly string robotDetectionServiceName;
        private readonly ISentryClient sentryClient;
        private readonly IRobotTWInsertServices robotAccountInsertServices;
        private readonly IRobotAIDetectionService robotDetectionService;
        private readonly IAppSettings appSettings;

        public RobotAccountInsertJobService(
            ISentryClient sentryClient,
            IRobotTWInsertServices robotAccountInsertServices,
            IRobotAIDetectionService robotDetectionService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.robotAccountInsertServices = robotAccountInsertServices;
            this.robotDetectionService = robotDetectionService;
            this.appSettings = appSettings;
            this.robotAccountServiceName = "TWRobotAccount";
            this.robotDetectionServiceName = "AIRobotDetection";
        }

        public Task<bool> ScanningTWRobotAccount(int batchExternalSize, int batchInternalSize)
        {
            var response = true;

            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    response = InsertTWRobot(batchExternalSize, batchInternalSize);
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

        public Task<bool> ScanningAIRobotClassification(int batchExternalSize, int batchInternalSize)
        {
            var response = true;

            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    response = InsertAIRobotClassification(batchExternalSize, batchInternalSize);
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

        private bool InsertTWRobot(int batchExternalSize, int batchInternalSize)
        {
            var isSuccess = robotAccountInsertServices.Start()
                                                      .GetLastTimespan()
                                                      .GetTWRobotUser()
                                                      .GetCustomerRole()
                                                      .GetRobotUserCategoryId(robotAccountServiceName)                                                      
                                                      .InsertTWRobotPackage(batchInternalSize, appSettings.AgencyApiUrl, new PushDataToExternalEntity
                                                          {
                                                              RoleGroup = RoleGroupType.Member,
                                                              IsUpdateDangerToMainDB = true,
                                                              ServiceName = robotAccountServiceName,
                                                              ExternalBatchSize = batchExternalSize,
                                                              IsWriteLog = appSettings.RobotAccountIsWriteLog,
                                                              IsSyncAFCData = false,
                                                              AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                                                              AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                                                          });

            if (isSuccess)
            {
                robotAccountInsertServices.UpdateLastTimespan();
            }

            return isSuccess;
        }

        public bool InsertAIRobotClassification(int batchExternalSize, int batchInternalSize)
        {
            var isSuccess = robotDetectionService.Start()
                                                 .GetRobotDetections(CTS_SystemParameters.RobotDetection_LastID)
                                                 .InsertAIRobotPackage(batchInternalSize, new PushDataToExternalEntity
                                                    {
                                                        RoleGroup = RoleGroupType.Member,
                                                        IsUpdateDangerToMainDB = true,
                                                        ServiceName = robotDetectionServiceName,
                                                        ExternalBatchSize = batchExternalSize,
                                                        IsWriteLog = appSettings.RobotAccountIsWriteLog,
                                                        IsSyncAFCData = false,
                                                        AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                                                        AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                                                    });

            if (isSuccess)
            {
                robotDetectionService.UpdateLastScannedId(CTS_SystemParameters.RobotDetection_LastID);
            }

            return isSuccess;
        }
    }

    #endregion Private Methods
}