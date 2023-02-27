namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IClassificationJobMonitorService
    {
        Task<bool> MonitorRealtimeInactiveCategory();
    }

    public class ClassificationJobMonitorService : IClassificationJobMonitorService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IRealtimeClassificationServices realtimeClassificationServices;
        private readonly IClassificationJobMonitorServices classificationJobMonitorServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly IAppSettings appSettings;

        public ClassificationJobMonitorService(
            ISentryClient sentryClient,
            IRealtimeClassificationServices realtimeClassificationServices,
            IClassificationJobMonitorServices classificationJobMonitorServices,
            ISystemParameterServices systemParameterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.realtimeClassificationServices = realtimeClassificationServices;
            this.classificationJobMonitorServices = classificationJobMonitorServices;
            this.systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
            this.serviceName = "ClassificationJobMonitor";
        }

        public Task<bool> MonitorRealtimeInactiveCategory()
        {
            try
            {
                var fromScannedTime = systemParameterServices.GetLastScannedDateTime(Constants.SystemParameters.JobMonitor_RealtimeLastScannedTime);
                var toScannedTime = fromScannedTime?.AddMinutes(60);

                if (fromScannedTime != null && toScannedTime <= DateTime.Now.AddMinutes(-60))
                {
                    var custIds = classificationJobMonitorServices.GetCustomerGeneralCategoryHasChangeForRunRealtime(fromScannedTime.Value, toScannedTime.Value);

                    if (custIds != null && custIds.Any())
                    {
                        var customers = custIds.ToList().Distinct().Select(c => new CustClassificationRealtimeEntity { CustId = c });

                        var custCategory = realtimeClassificationServices.GetCustomerGeneralCategoryForRunRealtime(customers);

                        classificationJobMonitorServices.CheckRealtimeInactiveCategory(custCategory)
                                                        .NotifyTelegram(appSettings.EvironmentName);
                    }

                    systemParameterServices.UpdateTimingExecution(toScannedTime.Value.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.SystemParameters.JobMonitor_RealtimeLastScannedTime);
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