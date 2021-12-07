namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerEvidenceInsertJobService
    {
        Task<bool> Execute(int internalBatchSize, int externalBatchSize);
    }

    public class CustomerEvidenceInsertJobService : ICustomerEvidenceInsertJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly ICustomerEvidenceInsertServices customerEvidenceInsertServices;
        private readonly ICustomerEvidenceSendMailServices customerEvidenceSendMailServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly IAppSettings appSettings;

        public CustomerEvidenceInsertJobService(
            ISentryClient sentryClient,
            ICustomerEvidenceInsertServices customerEvidenceInsertServices,
            ICustomerEvidenceSendMailServices customerEvidenceSendMailServices,
            ICustClassToMainDBService custClassToMainDBService,
            IAntiFraudCenterServices antiFraudCenterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.customerEvidenceInsertServices = customerEvidenceInsertServices;
            this.customerEvidenceSendMailServices = customerEvidenceSendMailServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.appSettings = appSettings;
            this.serviceName = "CustomerEvidenceInsert";
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
            IEnumerable<CustomerEvidenceFromFileEntity> custEvidenceFromFiles;

            do
            {
                custEvidenceFromFiles = customerEvidenceInsertServices.Start().GetCustomerEvidenceFromFile();

                if (custEvidenceFromFiles != null && custEvidenceFromFiles.Any())
                {
                    var groups = custEvidenceFromFiles.GroupBy(x => x.FileKey);

                    foreach (var custEviList in groups)
                    {
                        var isOverwrite = custEviList.ToList()[0].IsOverwrite;

                        InsertCustomerEvidence(custEviList, isOverwrite, internalBatchSize, externalBatchSize);
                    }
                }

                HandleLogError();

                Thread.Sleep(ImportEvidence_SleepTimes);

            } while (custEvidenceFromFiles != null && custEvidenceFromFiles.Any());
        }

        private void InsertCustomerEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidenceInFiles, bool isOverwrite, int internalBatchSize, int externalBatchSize)
        {
            var custEvidenceToUpdateComplete = new List<CustomerEvidenceFromFileEntity>();

            custEvidenceInFiles = custEvidenceInFiles.OrderBy(x => x.CTSCustID);

            var loopCustEvidences = customerEvidenceInsertServices.ChunkSize(custEvidenceInFiles.ToList(), internalBatchSize);

            foreach (var custEvidences in loopCustEvidences)
            {
                bool isImportSucess = true;

                if (isOverwrite)
                {
                    isImportSucess = customerEvidenceInsertServices.RemoveOldEvidence(custEvidences, out IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList);
                    isImportSucess = customerEvidenceInsertServices.SyncCustomerEvidenceToCTSCategory(custEvidences, EvidenceActionType.Remove, custEviRemovedList, isImportSucess);
                }

                isImportSucess = customerEvidenceInsertServices.InsertNewCustomerEvidence(custEvidences, isOverwrite, isImportSucess);
                isImportSucess = customerEvidenceInsertServices.SyncCustomerEvidenceToCTSCategory(custEvidences, EvidenceActionType.Insert, isImportSucess);

                if (isImportSucess)
                {
                    var custIds = custEvidences.Select(x => x.CustID).Distinct();

                    Parallel.Invoke(() =>
                    {
                        var isPushSuccess = custClassToMainDBService.PushDataToExternal(custIds, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = externalBatchSize,
                            FeedSoon88API = appSettings.Soon88FeedDataAPI,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = true,
                            DangerLevelType = DangerLevel.ABI,
                            IsWriteLog = false,
                            ServiceName = serviceName
                        });

                        if (isPushSuccess)
                        {
                            custEvidenceToUpdateComplete.AddRange(custEvidences);
                        }
                    },
                    () =>
                    {
                        antiFraudCenterServices.InsertAFCAccountsAsync(custIds, appSettings.AFCInsertAccountsAPI, appSettings.AFCUpdateAccountsAPI);
                    });
                }
            }

            if (custEvidenceToUpdateComplete.Any())
            {
                customerEvidenceInsertServices.UpdateBetLimitOfFlaggedCustomer(custEvidenceToUpdateComplete);
                customerEvidenceInsertServices.UpdateCompleteCustEvidence(custEvidenceToUpdateComplete, true);
            }
        }

        private void HandleLogError()
        {
            var processErrorList = customerEvidenceInsertServices.GetLogError();

            if (processErrorList != null && processErrorList.Any())
            {
                customerEvidenceSendMailServices.HandleLogError(processErrorList, appSettings.CustomerEvidenceIsSendMailToSPU, appSettings.EvironmentName, appSettings.CustomerEvidenceMailingToSPU);
            }
        }

        #endregion Private Methods
    }
}