namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWGroupBettingJobService
    {
        Task<bool> DetectGroupBettingTicket();

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

        public Task<bool> DetectGroupBettingTicket()
        {
            try
            {
                var rules = twGroupBettingService.GetAllRules();
                foreach (var item in rules.GroupBy(r => r.LimitSecond))
                {
                    var step = item.Key;
                    var fromTime = twGroupBettingService.GetLastScanTime(SystemParameters.TWGroupBetting_LastScanTime).AddSeconds(-step + 1);
                    var lastTransId = twGroupBettingService.GetLastTransID(SystemParameters.TWGroupBetting_LastTransID);
                    var toTime = fromTime.AddMinutes(1);
                    if (toTime < DateTime.Now)
                    {
                        var maxTransId = twGroupBettingService.ScanTicket(fromTime, toTime, step, lastTransId);
                        twGroupBettingService.CountGroupBettingTicket();

                        twGroupBettingService.UpdateLastScanTime(toTime, SystemParameters.TWGroupBetting_LastScanTime);
                        twGroupBettingService.UpdateMaxTransID(maxTransId, SystemParameters.TWGroupBetting_LastTransID);
                    }
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
