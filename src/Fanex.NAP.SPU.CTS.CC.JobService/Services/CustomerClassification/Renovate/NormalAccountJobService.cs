namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalAccountJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class NormalAccountJobService : INormalAccountJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly INormalAccountService normalAccountService;
        private readonly ICustClassToMainDBService custClassToMainDBService;       
        private readonly IAntiFraudCenterServices antiFraudCenterServices;

        public NormalAccountJobService(ISentryClient sentryClient,
            IAppSettings appSettings,
            INormalAccountService normalAccountService,
            ICustClassToMainDBService pushDataToExternalService,
            IAntiFraudCenterServices antiFraudCenterServices)
        {
            this.sentryClient = sentryClient;
            this.normalAccountService = normalAccountService;
            this.custClassToMainDBService = pushDataToExternalService;
            this.appSettings = appSettings;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.serviceName = "NormalAccountInsert";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                var isContinue = true;

                if (batchInternalSize <= 0)
                    batchInternalSize = 5000;

                do
                {
                    isContinue = JobRun(batchExternalSize, batchInternalSize);
                } while (isContinue);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool JobRun(int batchExternalSize, int batchInternalSize)
        {
            var normalAccountModel = normalAccountService.GetNormalAccounts();
            
            if (normalAccountModel.IsContinute)
            {
                var custChunks = normalAccountModel.NormalAccount.Chunk(batchInternalSize);

                var actions = new List<Action>();

                foreach (var custBatch in custChunks)
                {
                    actions.Add(() => InsertNormalAccount(custBatch, normalAccountModel.ScannedMaxId, batchExternalSize));
                }

                Parallel.Invoke(actions.ToArray());
            }

            return normalAccountModel.IsContinute;
        }

        private void InsertNormalAccount(IEnumerable<GetNormalAccountEntity> custs, long scannedMaxId, int batchExternalSize)
        {
            var twTaggedCusts = normalAccountService.GetGroupBettingAndRejectCustomer(custs);

            var custAfterCheckTWTagged = normalAccountService.MergeWithTWTaggedCust(custs, twTaggedCusts);

            var custIdAfterCheckTWTagged = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = normalAccountService.GetCustomerAssociationProblemAccount(custIdAfterCheckTWTagged);

            var finalNormalAcc = normalAccountService.MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

            var result = normalAccountService.InsertNormalAccount(finalNormalAcc, serviceName, appSettings.NormalAccountIsWriteLog);

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
                        IsWriteLog = appSettings.NormalAccountIsWriteLog,
                        ServiceName = serviceName
                    });

                    var custIdInsertResult = string.Join(",", finalNormalAcc.Select(x => x.CustId).Distinct());

                    normalAccountService.UpdateCompleteNormalAccountInsert(custIdInsertResult, scannedMaxId, isPushCCSuccess);
                },
                () =>
                {
                    antiFraudCenterServices.DeleteAFCAccounts(result.CustIds, FromFlow.CTSServices, appSettings.IsSyncAFCData, appSettings.AFCDeleteAccountURL);
                });
            }
        }
    }
}
