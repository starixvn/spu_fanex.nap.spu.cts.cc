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
        Task<bool> ScanTWGBTicket(int batchSize);

        Task<bool> UpdateTotalTWGBTicket();
    }

    public class TWGroupBettingJobService : ITWGroupBettingJobService
    {
        private readonly string serviceName;
        private readonly ITWGroupBettingService twGroupBettingService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public TWGroupBettingJobService(
            ISentryClient sentryClient,
            ITWGroupBettingService twGroupBettingService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.twGroupBettingService = twGroupBettingService;
            this.appSettings = appSettings;
            this.serviceName = "TWGroupBetting";
        }

        public Task<bool> ScanTWGBTicket(int batchSize)
        {
            try
            {
                var rules = twGroupBettingService.GetAllRules();

                foreach (var item in rules.GroupBy(r => r.LimitSecond))
                {
                    var lastTransId = twGroupBettingService.GetConfig_Int64(Main_SystemParameters.TWGroupBetting_ScanTicket_LastSequenceId);
                    var lastTransSec = twGroupBettingService.GetConfig_Int64(Main_SystemParameters.TWGroupBetting_LastTransSec);
                    var skipTransCount = twGroupBettingService.GetConfig_Int(Main_SystemParameters.TWGroupBetting_SkipTransCount);

                    var configs = new TWGroupBetting_ScanTicketCriteria
                    {
                        BatchSize = batchSize,
                        Step = item.Key,
                        LastSequenceId = lastTransId,
                        LastTransSec = lastTransSec,
                        SkipTransCount = skipTransCount
                    };

                    var outputParams = twGroupBettingService.ScanTicket(configs);

                    if (outputParams.LastSequenceId_New > 0)
                    {
                        twGroupBettingService.UpdateConfig_Int64(outputParams.LastSequenceId_New, Main_SystemParameters.TWGroupBetting_ScanTicket_LastSequenceId);
                        twGroupBettingService.UpdateConfig_Int64(outputParams.LastTransSec_New, Main_SystemParameters.TWGroupBetting_LastTransSec);
                        twGroupBettingService.UpdateConfig_Int(outputParams.SkipTransCount_New, Main_SystemParameters.TWGroupBetting_SkipTransCount);
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

        public Task<bool> UpdateTotalTWGBTicket()
        {
            try
            {
                var status = twGroupBettingService.GetConfigWithoutDefault(Main_SystemParameters.TWGroupBetting_CountTicket_Status);

                if (status != null && status == 0)
                {
                    var batchSize = twGroupBettingService.GetConfig_Int64(Main_SystemParameters.TWGroupBetting_CountTicket_BatchSize);
                    var LastSequenceId = twGroupBettingService.GetConfig_Int64(Main_SystemParameters.TWGroupBetting_CountTicket_LastSequenceId);

                    if (batchSize > 0 && LastSequenceId > 0)
                    {
                        twGroupBettingService.UpdateConfig_Int64(1, Main_SystemParameters.TWGroupBetting_CountTicket_Status);

                        long nextSequenceId = 0;

                        try
                        {
                            nextSequenceId = twGroupBettingService.UpdateTotalTWGBTicket((int)batchSize, LastSequenceId);

                            if (nextSequenceId > 0)
                            {
                                twGroupBettingService.UpdateConfig_Int64(nextSequenceId, Main_SystemParameters.TWGroupBetting_CountTicket_LastSequenceId);
                            }

                            twGroupBettingService.UpdateConfig_Int64(0, Main_SystemParameters.TWGroupBetting_CountTicket_Status);
                        }
                        catch (Exception ex)
                        {
                            string message = string.Empty;

                            if (nextSequenceId != 0)
                            {
                                message = $"CAUTION !!! To restart job, please monitor data, set SystemParamterID (137) = 0 and SystemParamterID (136) NextSequenceId = {nextSequenceId}\n";
                            }
                            else
                            {
                                twGroupBettingService.UpdateConfig_Int64(0, Main_SystemParameters.TWGroupBetting_CountTicket_Status);
                                message = $"Job was retried, LastSequenceId = {LastSequenceId} \n";
                            }

                            message += ex.Message;
                            throw new Exception(message, ex);
                        }
                    }
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                Utilities.NotifyTeamsMessage(ex.Message, "TWGB.CountTotalTicket", appSettings.EvironmentName, appSettings.EvironmentName == "LOCAL" ? MicrosoftTeams_GroupId_Local : MicrosoftTeams_JobMonitor_GroupId_Pro);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}