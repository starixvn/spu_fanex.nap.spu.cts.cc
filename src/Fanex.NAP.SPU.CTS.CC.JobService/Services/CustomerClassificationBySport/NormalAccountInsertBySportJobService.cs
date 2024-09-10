namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INormalAccountInsertBySportJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class NormalAccountInsertBySportJobService : INormalAccountInsertBySportJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountInsertBySportService normalAccountInsertService;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        private const int CategoryIdFromSpecialCC = -1; // for special cc cust no performance

        public NormalAccountInsertBySportJobService(ISentryClient sentryClient
            , INormalAccountInsertBySportService normalAccountInsertService
            , ICustClassToMainDBService custClassToMainDBService)
        {
            this.sentryClient = sentryClient;
            this.normalAccountInsertService = normalAccountInsertService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.serviceName = "InsertNormalAccountBySport";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                bool isContinue = true;

                if (batchInternalSize <= 0)
                    batchInternalSize = 5000;

                do
                {
                    isContinue = JobRun(batchExternalSize, batchInternalSize);

                } while (isContinue);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(this.serviceName, ex.Message, string.Empty, ex, sentryClient);
                
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool JobRun(int batchExternalSize, int batchInternalSize)
        {
            var isContinue = false;
            
            var newClassifications = normalAccountInsertService.GetNormalClassificationBySport();

            if (newClassifications.NormalAccount.IsNotNullOrEmpty()) 
            {
                var failedThreads = new ConcurrentBag<bool>();

                var custChunks = newClassifications.NormalAccount.Chunk(batchInternalSize).ToList();

                custChunks
                    .AsParallel()
                    .WithDegreeOfParallelism(4)
                    .ForAll(custBatch => InsertNormalClassificationBySport(custBatch, batchExternalSize, failedThreads));

                normalAccountInsertService.CompleteNormalAccountBySport(newClassifications.ScannedMaxId, failedThreads.IsEmpty);

                isContinue = true;
            }

            return isContinue;
        }

        private void InsertNormalClassificationBySport(IEnumerable<NormalAccountBySportEntity> custs, int batchExternalSize, ConcurrentBag<bool> result)
        {
            var insertNormalAccounts = custs.Where(x => x.CategoryId != CategoryIdFromSpecialCC);

            var specialCCAcounts = custs.Where(x => x.CategoryId == CategoryIdFromSpecialCC);

            var isInsertNormalSuccess = InsertNormaBySport(insertNormalAccounts, batchExternalSize);

            var isSpecialCCSucess = PushSpecialCCBySport(specialCCAcounts, batchExternalSize);

            if (!isInsertNormalSuccess || !isSpecialCCSucess)
            {
                result.Add(false);  
            }
        }

        private bool InsertNormaBySport(IEnumerable<NormalAccountBySportEntity> insertNormalAccounts, int batchExternalSize)
        {
            var isSuccess = true;

            if (insertNormalAccounts.IsNotNullOrEmpty())
            {
                var resultInsertNormalAccount = normalAccountInsertService.InsertNormalClassificationBySport(insertNormalAccounts, this.serviceName);

                var pushCC = resultInsertNormalAccount.CustInfos.Select(x => new InsertNormalAccountCustInfoEntity(x));

                isSuccess = string.IsNullOrEmpty(resultInsertNormalAccount?.ErrorMessage) &&
                            custClassToMainDBService.PushCCBySportToMainDB(pushCC, this.serviceName, batchExternalSize);
            }

            return isSuccess;
        }

        private bool PushSpecialCCBySport(IEnumerable<NormalAccountBySportEntity> specialCCAcounts, int batchExternalSize)
        {
            var isSuccess = true;

            if (specialCCAcounts.IsNotNullOrEmpty())
            {
                var pushCCListSpecialCC = specialCCAcounts.Select(x => new InsertNormalAccountCustInfoEntity(x));

                isSuccess = custClassToMainDBService.PushCCBySportToMainDB(pushCCListSpecialCC, this.serviceName, batchExternalSize);
            }

            return isSuccess;
        }
    }
}
