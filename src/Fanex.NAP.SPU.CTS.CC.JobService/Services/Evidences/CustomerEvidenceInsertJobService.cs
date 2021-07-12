namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
        private readonly IFeedSoon88Services feedSoon88Services;
        private readonly IAppSettings appSettings;

        public CustomerEvidenceInsertJobService(
            ISentryClient sentryClient,
            ICustomerEvidenceInsertServices customerEvidenceInsertServices,
            ICustomerEvidenceSendMailServices customerEvidenceSendMailServices,
             ICustClassToMainDBService custClassToMainDBService,
            IFeedSoon88Services feedSoon88Services,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.customerEvidenceInsertServices = customerEvidenceInsertServices;
            this.customerEvidenceSendMailServices = customerEvidenceSendMailServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.feedSoon88Services = feedSoon88Services;
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
                custEvidenceFromFiles = customerEvidenceInsertServices.GetCustomerEvidenceFromFile();

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
            } while (custEvidenceFromFiles != null && custEvidenceFromFiles.Any());
        }

        private void InsertCustomerEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidenceInFiles, bool isOverwrite, int internalBatchSize, int externalBatchSize)
        {
            var custEvidenceProcesseds = new List<CustomerEvidenceFromFileEntity>();

            custEvidenceInFiles = custEvidenceInFiles.OrderBy(x => x.CTSCustID);

            var loopCustEvidences = customerEvidenceInsertServices.ChunkSize(custEvidenceInFiles.ToList(), internalBatchSize);

            foreach (var custEvidences in loopCustEvidences)
            {
                bool isSuccess = true;

                try
                {
                    if (isOverwrite)
                    {
                        isSuccess = customerEvidenceInsertServices.RemoveOldEvidence(custEvidences, out IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList);
                        isSuccess = customerEvidenceInsertServices.SyncCustomerEvidenceToCTSCategory(custEvidences, 2, custEviRemovedList, isSuccess);
                    }

                    isSuccess = customerEvidenceInsertServices.InsertNewCustomerEvidence(custEvidences, isOverwrite, isSuccess);
                    isSuccess = customerEvidenceInsertServices.SyncCustomerEvidenceToCTSCategory(custEvidences, 0, isSuccess);

                    if (isSuccess)
                    {
                        custEvidenceProcesseds.AddRange(custEvidences);
                    }

                    customerEvidenceInsertServices.UpdateProcessedCustEvidence(custEvidences, isSuccess);
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(serviceName, "InsertCustomerEvidence throws exception:", custEvidences, ex, sentryClient);
                }
            }

            if (custEvidenceProcesseds.Any())
            {
                var custIds = custEvidenceProcesseds.Select(x => x.CustID).Distinct();

                custClassToMainDBService.PushDataToExternal(custIds, new PushDataToExternalEntity
                {
                    ExternalBatchSize = externalBatchSize,
                    FeedSoon88API = appSettings.Soon88FeedDataAPI,
                    IsPushCCToSoon88 = true,
                    IsUpdateDangerToMainDB = false,
                    IsWriteLog = false,
                    ServiceName = serviceName
                });
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