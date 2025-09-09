namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRealtimeTWInitialSmartBySportJobService
    {
        Task<bool> Execute(int internalBatchSize, int externalBatchSize);
    }

    public class RealtimeTWInitialSmartBySportJobService : IRealtimeTWInitialSmartBySportJobService
    {
        private const string ServiceName = "Realtime TW Initial Smart By Sport JobService";
        private readonly IAppSettings _appSettings;
        private readonly ITWInitialSmartBySportService _service;

        public RealtimeTWInitialSmartBySportJobService(IAppSettings appSettings, ITWInitialSmartBySportService service)
        {
            _appSettings = appSettings;
            _service = service;
        }

        public Task<bool> Execute(int internalBatchSize, int externalBatchSize)
        {
            try
            {
                JobRun(internalBatchSize, externalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, ex.Message, string.Empty, ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int internalBatchSize, int externalBatchSize)
        {
            bool isContinue;

            var paras = new PushDataToExternalEntity
            {
                RoleGroup = RoleGroupType.Member,
                ExternalBatchSize = externalBatchSize,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = _appSettings.ProblemAccountIsWriteLog,
                ServiceName = ServiceName
            };

            do
            {
                isContinue = _service
                                .Start()
                                .GetCustInitialSmartBySport(internalBatchSize)
                                .InsertTWInitialSmartClassification(paras)
                                .UpdateLastProcessed();

            } while (isContinue);
        }
    }
}
