namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAffectedDownlineCustomerClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class AffectedDownlineCustomerClassificationJobService : IAffectedDownlineCustomerClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IAffectedDownlineCustomerClassificationService downlineCustomerClassificationService;

        public AffectedDownlineCustomerClassificationJobService(
            ISentryClient sentryClient,
            IAppSettings appSettings,
            IAffectedDownlineCustomerClassificationService downlineCustomerClassificationService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.downlineCustomerClassificationService = downlineCustomerClassificationService;
            this.serviceName = "AffectedDownlineCustomerClassification";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                return Task.FromResult(JobRun(batchSize));
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }
        }

        #region Private Methods

        private bool JobRun(int batchSize)
        {
            var paramaters = new RescanParametersEntity()
            {
                IsRescanPA = false,
                IsPushCCToSoon88 = true,
                IsFromQueue = true,
                IsUpdateDangerToMainDB = true,
                FunctionId = FunctionId.DownlineCustomerClassification,
                ServiceName = serviceName,
                InternalBatchSize = batchSize,
                ExternalBatchSize = batchSize,
                IsWriteLog = false,
                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                IsSyncAFCData = appSettings.IsSyncAFCData,
                AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
            };

            return downlineCustomerClassificationService.Start()
                                                        .GetCustomerClassificationFromQueue()
                                                        .CustomerClassification(paramaters)
                                                        .CompleteCustomerClassificationFromQueue();
}
    }

    #endregion Private Methods
}