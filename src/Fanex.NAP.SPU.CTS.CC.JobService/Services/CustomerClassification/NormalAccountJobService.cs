namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INormalAccountJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);

        Task<bool> UpdateWriteLogSetting(string serviceName, bool isWriteLog);
    }

    public class NormalAccountJobService : INormalAccountJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountServices normalAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAppSettings appSettings;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;

        public NormalAccountJobService(
            ISentryClient sentryClient,
            INormalAccountServices normalAccountServices,
            ICustClassToMainDBService pushDataToExternalService,
            IAntiFraudCenterServices antiFraudCenterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.normalAccountServices = normalAccountServices;
            this.custClassToMainDBService = pushDataToExternalService;
            this.appSettings = appSettings;
            this.antiFraudCenterServices = antiFraudCenterServices;
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

        public Task<bool> UpdateWriteLogSetting(string serviceName, bool isWriteLog)
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
            var newClassification = normalAccountServices.GetNewClassification(serviceName, appSettings.NormalAccountIsWriteLog);
            
            if (newClassification.GeneralCustClassifications != null && newClassification.GeneralCustClassifications.Any())
            {                
                var generalClassificationChunk = newClassification.GeneralCustClassifications.Chunk(batchInternalSize).ToList();

                foreach (var listClassification in generalClassificationChunk)
                {
                    var custAssociationPA = normalAccountServices.GetCustomerAssociationProblemAccount(listClassification);                    

                    var custClassificationAfterCheckPA = normalAccountServices.MergeWithTaggedCust(listClassification, custAssociationPA);

                    var custGroupBettingAndReject = normalAccountServices.GetGroupBettingAndRejectCustomer(custClassificationAfterCheckPA.Where(c => c.TaggingID == 0));
                    
                    var finalCustClassification = normalAccountServices.MergeWithTaggedCust(custClassificationAfterCheckPA, custGroupBettingAndReject);                    

                    var result = normalAccountServices.InsertMainCustomerClassification(finalCustClassification, serviceName, appSettings.NormalAccountIsWriteLog);

                    if (string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        Parallel.Invoke(() =>
                        {
                            var isPushCCSuccess = custClassToMainDBService.PushDataToExternal(result.CustIds, new PushDataToExternalEntity
                            {
                                ExternalBatchSize = batchExternalSize,
                                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                                IsPushCCToSoon88 = true,
                                IsUpdateDangerToMainDB = true,
                                DangerLevelType = Constants.DangerLevel.Original,
                                IsWriteLog = appSettings.NormalAccountIsWriteLog,
                                ServiceName = serviceName
                            });

                            normalAccountServices.UpdateAppliedStatusClassification(listClassification, isPushCCSuccess);
                        },
                        () =>
                        {
                            antiFraudCenterServices.DeleteAFCAccountsAsync(result.CustIds, appSettings.AFCDeleteAccountsAPI);
                        });

                    }
                }
            }

            if (newClassification.IsContinute)
            {
                JobRun(batchExternalSize, batchInternalSize);
            }
        }
    }

    #endregion Private Methods
}