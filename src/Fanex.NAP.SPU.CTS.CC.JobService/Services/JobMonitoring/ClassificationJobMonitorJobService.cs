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

        Task<bool> MonitorNormalPools(int threshold);
    }

    public class ClassificationJobMonitorJobService : IClassificationJobMonitorJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IClassificationJobMonitorService classificationJobMonitorService;
        private readonly ISystemParameterServices systemParameterServices;

        public ClassificationJobMonitorJobService(
            ISentryClient sentryClient,
            IClassificationJobMonitorService classificationJobMonitorService,
            ISystemParameterServices systemParameterServices)
        {
            this.sentryClient = sentryClient;
            this.classificationJobMonitorService = classificationJobMonitorService;
            this.systemParameterServices = systemParameterServices;
            this.serviceName = "ClassificationJobMonitor";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        public Task<bool> MonitorRealtimeInactiveCategory(int batchSize)
        {
            try
            {
                var fromScannedTime = systemParameterServices.GetLastScannedDateTime(CTS_SystemParameters.JobMonitor_RealtimeLastScannedTime);
                var toScannedTime = fromScannedTime?.AddMinutes(60);

                if (fromScannedTime != null && toScannedTime <= DateTime.Now.AddMinutes(-60))
                {
                    var custIds = classificationJobMonitorService.GetRealtimeCustomerPlaceNewBet(fromScannedTime.Value, toScannedTime.Value);

                    if (custIds != null && custIds.Any())
                    {
                        var custCategory = classificationJobMonitorService.GetCustomerCategoryToMonitor(custIds, batchSize);

                        classificationJobMonitorService.FilterInvalidCustomerCategory(custCategory)
                                                       .InsertInvalidCategory(batchSize);

                        classificationJobMonitorService.ClearRealtimeCustomerPlaceNewBets(fromScannedTime.Value, toScannedTime.Value);
                    }

                    systemParameterServices.UpdateTimingExecution(toScannedTime.Value.ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.JobMonitor_RealtimeLastScannedTime);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        public Task<bool> MonitorNormalPools(int threshold)
        {
            try
            {
                var normalPool = classificationJobMonitorService.GetNormalPoolMonitoring();

                if (normalPool != null && normalPool.Any())
                {
                    foreach (var item in normalPool)
                    {
                        if (item.NumberOfRecord > threshold)
                        {
                            classificationJobMonitorService.NotifyExcessivePools(item.NumberOfRecord, item.MapIssueTypeID, 500, threshold);
                        }
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