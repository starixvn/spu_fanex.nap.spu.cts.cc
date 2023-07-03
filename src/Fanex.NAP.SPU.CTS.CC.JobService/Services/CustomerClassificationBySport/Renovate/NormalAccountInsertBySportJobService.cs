namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
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
        private readonly IAppSettings appSettings;
        private readonly INormalAccountInsertBySportService normalAccountService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
       

        public NormalAccountInsertBySportJobService()
        {

        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }

            return Task.FromResult(true);
        }

        private bool JobRun(int batchExternalSize, int batchInternalSize)
        {
            var isContinue = false;

            var newClassifications = normalAccountService.GetNormalAccount();

            if (newClassifications.NormalAccount != null && newClassifications.NormalAccount.Any())
            {
                var newClassificationChunks = newClassifications.NormalAccount.Chunk(batchInternalSize).ToList();

                foreach (var classifications in newClassificationChunks)
                {
                    //var insertNAResult = normalAccountService.InsertNormalAccount(classifications, this.serviceName, appSettings.NormalAccountBySportIsWriteLog);

                    //if (string.IsNullOrEmpty(insertNAResult.ErrorMessage))
                    //{
                    //    var isPushCCSuccess = custClassToMainDBService.PushCCBySportToMainDB(insertNAResult.CustInfos, this.serviceName, batchExternalSize);

                    //    normalAccountService.CompleteNormalAccountBySport(classifications, isPushCCSuccess);
                    //}
                }

                isContinue = true;
            }

            return isContinue;
        }
    }
}
