namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INewCustomerClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class NewCustomerClassificationJobService : INewCustomerClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly INewCustomerClassificationServices newCTSClassificationServices;
        private readonly IAppSettings appSettings;

        public NewCustomerClassificationJobService(
            ISentryClient sentryClient,
            IAppSettings appSettings,
            INewCustomerClassificationServices newCTSClassificationServices)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.newCTSClassificationServices = newCTSClassificationServices;
            this.serviceName = "NewCTSCustomerClassification";
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
                IsFromQueue = false,
                IsUpdateDangerToMainDB = true,
                FunctionId = FunctionId.InsertNewCustJob,
                ServiceName = "NewCTSCustomerClassification",
                InternalBatchSize = batchSize,
                ExternalBatchSize = batchSize,
                IsWriteLog = false,
                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                IsSyncAFCData = appSettings.IsSyncAFCData,
                AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
            };

            var isSuccess = newCTSClassificationServices.Start()
                                                        .GetCategoryForNewCTSCustomer()
                                                        .InsertVVIPForNewCustomers(paramaters)
                                                        .InsertPAForNewCustomers(paramaters)
                                                        .ResanNormalForNewCustomers(paramaters)
                                                        .UpdateComplete();
            return isSuccess;
       }
    }

    #endregion Private Methods
}