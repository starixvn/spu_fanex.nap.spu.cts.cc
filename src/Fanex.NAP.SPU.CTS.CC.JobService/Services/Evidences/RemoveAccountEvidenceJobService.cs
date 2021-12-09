namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRemoveAccountEvidenceJobService
    {
        Task<bool> Execute(int internalBatchSize, int externalBatchSize);
    }

    public class RemoveAccountEvidenceJobService : IRemoveAccountEvidenceJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IRemoveAccountEvidenceServices removeAccountEvidenceServices;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private readonly IAppSettings appSettings;

        public RemoveAccountEvidenceJobService(
            ISentryClient sentryClient,
            IRemoveAccountEvidenceServices removeAccountEvidenceServices,
            IRescanCustClassificationService rescanCustClassificationService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.removeAccountEvidenceServices = removeAccountEvidenceServices;
            this.rescanCustClassificationService = rescanCustClassificationService;
            this.appSettings = appSettings;
            this.serviceName = "RemoveAccountEvidence";
        }

        public Task<bool> Execute(int internalBatchSize, int externalBatchSize)
        {
            try
            {
                if (internalBatchSize > 0 && externalBatchSize > 0)
                {
                    JobRun(internalBatchSize, externalBatchSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (internalBatchSize, externalBatchSize), new NullReferenceException(), sentryClient);
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

        private void JobRun(int internalBatchSize, int externalBatchSize)
        {
            IEnumerable<CustomerEvidenceQueueEntity> customerEvidenceFromQueue;

            customerEvidenceFromQueue = removeAccountEvidenceServices.GetCustomerEvidenceQueue();

            if (customerEvidenceFromQueue != null && customerEvidenceFromQueue.Any())
            {
                var isSuccess = true;

                var result = removeAccountEvidenceServices
                                .InitialData()
                                .RemoveCustEvidenceFromQueue(customerEvidenceFromQueue, RemoveEvidences_ClearActionId, serviceName, sentryClient)
                                .GetCustomerToUpdateCC();

                if (result.IsRemoveSuccess && result.CustIds != null && result.CustIds.Any())
                {
                    var paras = new RescanParametersEntity
                    {
                        CustIds = result.CustIds,
                        FunctionId = FunctionId.RemoveMultiEvidencesWeb,
                        IsCheckPAFromNAP = true,
                        IsPushCCToSoon88 = true,
                        IsUpdateDangerToMainDB = true,
                        DangerLevelType = DangerLevel.ABI,
                        ServiceName = serviceName,
                        FeedSoon88API = appSettings.Soon88FeedDataAPI,
                        IsWriteLog = appSettings.RemoveAccountEvidenceIsWriteLog,
                        InternalBatchSize = internalBatchSize,
                        ExternalBatchSize = externalBatchSize,
                        IsSyncAFCData = appSettings.IsSyncAFCData,
                        AFCInsertAccountsAPI = appSettings.AFCInsertAccountsAPI,
                        AFCUpdateAccountsAPI = appSettings.AFCUpdateAccountsAPI,
                        AFCDeleteAccountsAPI = appSettings.AFCDeleteAccountsAPI
                    };

                    isSuccess = rescanCustClassificationService.RescanCustomerClassify(paras);
                }

                if (result.IsRemoveSuccess && isSuccess)
                {
                    removeAccountEvidenceServices.CheckCleanCustFlaggedEvidence(customerEvidenceFromQueue)
                                                 .UpdateBetLimitOfUnflaggedCustomer(serviceName, sentryClient)
                                                 .UpdateCustomerEvidenceComplete(customerEvidenceFromQueue, serviceName, sentryClient);
                }

                JobRun(internalBatchSize, externalBatchSize);
            }
        }

        #endregion Private Methods
    }
}