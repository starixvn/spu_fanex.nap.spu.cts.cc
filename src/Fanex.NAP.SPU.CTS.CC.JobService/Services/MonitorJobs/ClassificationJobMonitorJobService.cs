namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IClassificationJobMonitorJobService
    {
        Task<bool> MonitorRealtimeInactiveCategory(int batchSize);
    }

    public class ClassificationJobMonitorJobService : IClassificationJobMonitorJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IClassificationJobMonitorService classificationJobMonitorService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly IAppSettings appSettings;

        public ClassificationJobMonitorJobService(
            ISentryClient sentryClient,
            IClassificationJobMonitorService classificationJobMonitorService,
            ISystemParameterServices systemParameterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.classificationJobMonitorService = classificationJobMonitorService;
            this.systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
            this.serviceName = "ClassificationJobMonitor";
        }

        public Task<bool> MonitorRealtimeInactiveCategory(int batchSize)
        {
            try
            {
                var fromScannedTime = systemParameterServices.GetLastScannedDateTime(SystemParameters.JobMonitor_RealtimeLastScannedTime);
                var toScannedTime = fromScannedTime?.AddMinutes(60);

                if (fromScannedTime != null && toScannedTime <= DateTime.Now.AddMinutes(-60))
                {
                    var custIds = classificationJobMonitorService.GetRealtimeCustomerPlaceNewBet(fromScannedTime.Value, toScannedTime.Value);

                    if (custIds != null && custIds.Any())
                    {
                        var custCategory = classificationJobMonitorService.GetCustomerCategoriesToMonitor(custIds, batchSize);

                        classificationJobMonitorService.CheckRealtimeCustomerCategory(custCategory)
                                                       .InsertMonitorInvalidCustomerCategory(batchSize, toScannedTime.Value)
                                                       .NotifyTelegram(appSettings.EvironmentName, fromScannedTime.Value, toScannedTime.Value);

                        classificationJobMonitorService.ClearRealtimeCustomerPlaceNewBets(fromScannedTime.Value, toScannedTime.Value);
                    }

                    systemParameterServices.UpdateTimingExecution(toScannedTime.Value.ToString(CONVERT_TO_SQL_DATETIME), SystemParameters.JobMonitor_RealtimeLastScannedTime);
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