namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INormalAccountJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);

        Task<bool> UpdateWriteLogSetting(bool isWriteLog);
    }

    public class NormalAccountJobService : INormalAccountJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountServices normalAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IFeedSoon88Services feedSoon88Services;
        private readonly IAppSettings appSettings;
        private readonly bool isWriteLog;

        public NormalAccountJobService(
            ISentryClient sentryClient,
            INormalAccountServices normalAccountServices,
            ICustClassToMainDBService pushDataToExternalService,
            IFeedSoon88Services feedSoon88Services,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.normalAccountServices = normalAccountServices;
            this.custClassToMainDBService = pushDataToExternalService;
            this.feedSoon88Services = feedSoon88Services;
            this.appSettings = appSettings;
            this.isWriteLog = appSettings.NormalAccountIsWriteLog;
            this.serviceName = "NormalAccountInsert";
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

        public Task<bool> UpdateWriteLogSetting(bool isWriteLog)
        {
            try
            {
                bool result = appSettings.UpdateIsWriteLogSetting(serviceName, isWriteLog);

                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                sentryClient.CaptureException(e);
                return Task.FromResult(false);
            }
        }

        #region Private Methods

        private void JobRun(int batchExternalSize, int batchInternalSize)
        {
            GeneralCustomerClassificationEnity newClassification;

            do
            {
                newClassification = normalAccountServices.GetNewClassification(serviceName, isWriteLog);

                if (newClassification.GeneralCustClassifications != null && newClassification.GeneralCustClassifications.Any())
                {
                    var generalClassificationChunk = newClassification.GeneralCustClassifications.Chunk(batchInternalSize).ToList();

                    foreach (var listClassification in generalClassificationChunk)
                    {
                        var custAssociationPA = normalAccountServices.GetCustomerAssociationProblemAccount(listClassification);

                        var custIds = normalAccountServices.InsertMainCustomerClassification(listClassification, custAssociationPA, serviceName, isWriteLog);

                        custClassToMainDBService.PushDataToExternal(custIds, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = batchExternalSize,
                            FeedSoon88API = appSettings.Soon88FeedDataAPI,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = true,
                            IsWriteLog = isWriteLog,
                            ServiceName = serviceName
                        });
                    }
                }
            } while (newClassification.IsContinute);
        }
    }

    #endregion Private Methods
}