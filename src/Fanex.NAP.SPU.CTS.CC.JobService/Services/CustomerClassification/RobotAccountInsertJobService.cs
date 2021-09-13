namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface IRobotAccountInsertJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class RobotAccountInsertJobService : IRobotAccountInsertJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IRobotAccountInsertServices robotAccountInsertServices;
        private readonly IAppSettings appSettings;

        public RobotAccountInsertJobService(
            ISentryClient sentryClient,
            IRobotAccountInsertServices robotAccountInsertServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.robotAccountInsertServices = robotAccountInsertServices;
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
            var isSuccess = robotAccountInsertServices.Start()
                                                      .GetLastTimespan()
                                                      .GetRobotUser()
                                                      .Process(new InsertRobotParaEntity
                                                      {
                                                          BatchExternalSize = batchExternalSize,
                                                          BatchInternalSize = batchInternalSize,
                                                          ServiceName = serviceName,
                                                          Soon88FeedDataAPI = appSettings.Soon88FeedDataAPI,
                                                          IsWriteLog = appSettings.RobotAccountIsWriteLog
                                                      });


            if (isSuccess)
            {
                robotAccountInsertServices.UpdateLastTimespan();
            }

            return isSuccess;
        }
    }

    #endregion Private Methods
}