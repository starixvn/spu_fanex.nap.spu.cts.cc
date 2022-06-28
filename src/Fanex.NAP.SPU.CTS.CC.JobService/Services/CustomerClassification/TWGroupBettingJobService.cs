namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWGroupBettingJobService
    {
        Task<bool> DetectGroupBettingTicket(int batchSize);

        Task<bool> CountTotalTicket();
    }

    public class TWGroupBettingJobService : ITWGroupBettingJobService
    {
        private readonly string serviceName;
        private readonly ITWGroupBettingService twGroupBettingService;
        private readonly ISentryClient sentryClient;

        public TWGroupBettingJobService(
            ISentryClient sentryClient,
            ITWGroupBettingService twGroupBettingService)
        {
            this.sentryClient = sentryClient;
            this.twGroupBettingService = twGroupBettingService;
            this.serviceName = "TWGroupBetting";
        }

        public Task<bool> DetectGroupBettingTicket(int batchSize)
        {
            try
            {
                var rules = twGroupBettingService.GetAllRules();
                foreach (var item in rules.GroupBy(r => r.LimitSecond))
                {
                    var step = item.Key;
                    var lastTransId = twGroupBettingService.GetLastTransID(SystemParameters.TWGroupBetting_LastTransID);
                    var maxTransId = twGroupBettingService.ScanTicket(batchSize, step, lastTransId);

                    twGroupBettingService.CountGroupBettingTicket();
                    twGroupBettingService.UpdateMaxTransID(maxTransId, SystemParameters.TWGroupBetting_LastTransID);

                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> CountTotalTicket()
        {
            try
            {
                twGroupBettingService.CountTotalTicket();
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}