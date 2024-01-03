namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IIncorrectDataMonitoringJobService
    {
        Task<bool> NotififyIncorrectDataMonitoring();
    }

    public class IncorrectDataMonitoringJobService : IIncorrectDataMonitoringJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IIncorrectDataMonitoringService monitoringService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly IAppSettings appSettings;

        public IncorrectDataMonitoringJobService(
            ISentryClient sentryClient,
            IIncorrectDataMonitoringService monitoringService,
            ISystemParameterServices systemParameterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.monitoringService = monitoringService;
            this.systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
            this.serviceName = "IncorrectDataMonitoring";
        }

        public Task<bool> NotififyIncorrectDataMonitoring()
        {
            try
            {
                var incorrectData = monitoringService.GetIncorrectDataMonitoring();

                if (incorrectData != null && incorrectData.Any())
                {
                    //NotifyTelegram

                    systemParameterServices.UpdateTimingExecution(incorrectData.Max(c=>c.ID).ToString(), SystemParameters.JobMonitoring_IncorrectData_LastScanID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        //private static void NotifyTelegram(IEnumerable<long> custIds, string environment, DateTime scannedTimeFrom, DateTime scannedTimeTo, JobMonitorType type)
        //{
        //    if (custIds.Any())
        //    {
        //        var message = string.Empty;

        //        custIds = custIds.Distinct();

        //        switch (type)
        //        {
        //            case JobMonitorType.Realtime_NullCategory:
        //                {
        //                    message = $"Please kindly check the following custid with category NULL - {{Total custid: {custIds.Count()}}} - {string.Join(',', custIds.Take(50))} - " +
        //                              $"Scan from  \"{scannedTimeFrom:dd/MM/yyyy HH:mm:ss}\" to  \"{scannedTimeTo:dd/MM/yyyy HH:mm:ss}\"";
        //                    break;
        //                }
        //            case JobMonitorType.Realtime_NewPlaceBetInactiveCategory:
        //                {
        //                    message = $"Please kindly check the following custid NOT RE-ACTIVATE - {{Total custid: {custIds.Count()}}} - {string.Join(',', custIds.Take(50))} - " +
        //                              $"Scan from \"{scannedTimeFrom:dd/MM/yyyy HH:mm:ss}\" to \"{scannedTimeTo:dd/MM/yyyy HH:mm:ss}\"";
        //                    break;
        //                }
        //        }

        //        Utilities.NotifyTelegramMessage(message, "Monitoring - Realtime Classification", environment, environment == "LOCAL" ? Telegram_GroupId_Local : Telegram_JobMonitor_GroupId_Pro);
        //    }
        //}
    }
}