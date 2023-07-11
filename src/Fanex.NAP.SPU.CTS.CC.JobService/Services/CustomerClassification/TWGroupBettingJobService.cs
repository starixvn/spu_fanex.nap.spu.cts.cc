namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria;
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
                    var lastTransId = twGroupBettingService.GetConfig_Int64(SystemParameters.TWGroupBetting_ScanTicket_LastSequenceId);
                    var lastTransSec = twGroupBettingService.GetConfig_Int64(SystemParameters.TWGroupBetting_LastTransSec);
                    var skipTransCount = twGroupBettingService.GetConfig_Int(SystemParameters.TWGroupBetting_SkipTransCount);

                    var configs = new TWGroupBetting_ScanTicketCriteria
                    {
                        BatchSize = batchSize,
                        Step = item.Key,
                        LastSequenceId = lastTransId,
                        LastTransSec = lastTransSec,
                        SkipTransCount = skipTransCount
                    };
                    var outputParams = twGroupBettingService.ScanTicket(configs);
                    
                    twGroupBettingService.CountGroupBettingTicket();
                    if (outputParams.LastSequenceId_New > 0)
                    {
                        twGroupBettingService.UpdateConfig_Int64(outputParams.LastSequenceId_New, SystemParameters.TWGroupBetting_ScanTicket_LastSequenceId);
                        twGroupBettingService.UpdateConfig_Int64(outputParams.LastTransSec_New, SystemParameters.TWGroupBetting_LastTransSec);
                        twGroupBettingService.UpdateConfig_Int(outputParams.SkipTransCount_New, SystemParameters.TWGroupBetting_SkipTransCount);
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
                var batchSize = twGroupBettingService.GetConfig_Int64(SystemParameters.TWGroupBetting_CountTicket_BatchSize);
                var LastSequenceId = twGroupBettingService.GetConfig_Int64(SystemParameters.TWGroupBetting_CountTicket_LastSequenceId);

                if (batchSize > 0 && LastSequenceId> 0)
                {
                    var nextLastSequenceId = twGroupBettingService.CountTotalTicket((int)batchSize, LastSequenceId);

                    if (nextLastSequenceId > 0)
                    {
                        twGroupBettingService.UpdateConfig_Int64(nextLastSequenceId, SystemParameters.TWGroupBetting_CountTicket_LastSequenceId);
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
    }
}