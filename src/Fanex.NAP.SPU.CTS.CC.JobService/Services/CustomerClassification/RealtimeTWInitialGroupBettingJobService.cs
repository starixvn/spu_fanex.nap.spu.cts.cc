namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRealtimeTWInitialGroupBettingJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class RealtimeTWInitialGroupBettingJobService : IRealtimeTWInitialGroupBettingJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly ITWInitialGroupBettingService twInitialGBService;

        public RealtimeTWInitialGroupBettingJobService(ISentryClient sentryClient, IAppSettings appSettings, ITWInitialGroupBettingService twInitialGBService)
        {
            this.sentryClient = sentryClient;
            this.twInitialGBService = twInitialGBService;        
            this.appSettings = appSettings;
            this.serviceName = "Realtime TW Initial Group Betting JobService";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                JobRun(batchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchSize)
        {
            bool isContinue;

            var paras = new PushDataToExternalEntity
            {
                RoleGroup = RoleGroupType.Member,
                ExternalBatchSize = batchSize,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                ServiceName = serviceName
            };

            do
            {
                var twInitialGB = twInitialGBService.GetChanges();
              
                isContinue = twInitialGB.CustInfos != null && twInitialGB.CustInfos.Any();

                if (isContinue)
                {
                    twInitialGBService.InsertTWInitalGBClassification(twInitialGB.CustInfos, paras);

                    twInitialGBService.UpdateNextScannedTime(twInitialGB.NextScannedDateTime, twInitialGB.NextScannedCustID);
                }

            } while (isContinue);
        }

    }
}
