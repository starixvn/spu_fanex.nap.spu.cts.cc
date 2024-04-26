namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Concurrent;
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
                var failedThreads = new ConcurrentBag<bool>();
                
                var custChunks = normalAccountModel.NormalAccount.Chunk(batchInternalSize);

                custChunks
                    .AsParallel()
                    .WithDegreeOfParallelism(5)
                    .ForAll(custBatch => InsertNormalAccount(custBatch, batchExternalSize, failedThreads));

                normalAccountService.CompleteNormalAccountInsert(normalAccountModel.ScannedMaxId, failedThreads.IsEmpty);
            }

            return normalAccountModel.IsContinute;
        }

        private void InsertNormalAccount(IEnumerable<GetNormalAccountEntity> custs, int batchExternalSize, ConcurrentBag<bool> failedThreads)
        {            
            var finalNormalAcc = normalAccountService.MergeWithTWTaggedCustAndCTSTaggedCust(custs);

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

                    if (!isPushCCSuccess)
                    {
                        failedThreads.Add(false);
                    }
                },
                () =>
                {
                    antiFraudCenterServices.DeleteAFCAccounts(result.CustIds, FromFlow.CTSServices, appSettings.IsSyncAFCData, appSettings.AFCDeleteAccountURL);
                });
            }
        }
    }
}
