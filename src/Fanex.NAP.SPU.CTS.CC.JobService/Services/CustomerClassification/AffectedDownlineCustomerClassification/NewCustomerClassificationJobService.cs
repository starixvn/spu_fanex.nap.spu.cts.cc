namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
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
        private readonly IDataLogTrackingServices dataLogTrackingServices;
        private readonly IAppSettings appSettings;

        public NewCustomerClassificationJobService(
            ISentryClient sentryClient,
            IAppSettings appSettings,
            INewCustomerClassificationServices newCTSClassificationServices,
            IDataLogTrackingServices dataLogTrackingServices)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.newCTSClassificationServices = newCTSClassificationServices;
            this.dataLogTrackingServices = dataLogTrackingServices;
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

                dataLogTrackingServices.LogErrorData(ex, serviceName);

                return Task.FromResult(false);
            }
        }

        #region Private Methods

        private bool JobRun(int batchSize)
        {
            var paramaters = new RescanParametersEntity
            {
                IsToQueue = false,
                FunctionId = FunctionId.InsertNewCustJob,
                RescanNormalType = RescanNormalType.Auto_Customer,
                NewCustomerClassificationAgencyURL = appSettings.NewCustomerClassificationAgencyURL,
                InternalBatchSize = batchSize,
                ServiceName = serviceName,
                PushDataExternal = new PushDataToExternalEntity
                {
                    RoleGroup = RoleGroupType.Member,
                    IsUpdateDangerToMainDB = true,
                    ExternalBatchSize = batchSize,
                    ServiceName = serviceName,
                    IsSyncAFCData = appSettings.IsSyncAFCData,
                    AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                    AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                },
            };

            var isSuccess = newCTSClassificationServices.Start()
                                                       .GetCategoryForNewCTSCustomer()
                                                       .NewCustomerClassificationAgency(paramaters)
                                                       .InsertVVIPForNewCustomers(paramaters)
                                                       .InsertPAForNewCustomers(paramaters)
                                                       .ResanNormalForNewCustomers(paramaters)
                                                       .ResanNormalBySportForNewCustomers()
                                                       .UpdateComplete();
            return isSuccess;
        }
    }

    #endregion Private Methods
}