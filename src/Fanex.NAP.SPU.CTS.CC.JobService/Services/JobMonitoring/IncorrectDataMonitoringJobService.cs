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
                bool isContinue = true;

                do
                {
                    var result = monitoringService.GetIncorrectDataMonitoring();

                    isContinue = result.IsContinue;

                    if (result.InvalidDataList != null && result.InvalidDataList.Any())
                    {
                        monitoringService.NotifyTelegram(result.InvalidDataList, appSettings.EvironmentName);

                        var inValidIssueType = result.InvalidDataList.Where(c => c.IsIssueTypeValid == false);

                        if (inValidIssueType.Any())
                        {
                            Utilities.LogSentryError(serviceName, $"The are undefined causes of incorrect data, because of invalid IssueTypeID. Please check more detail with list ID (table CTS_DataCenter.JobMonitoring_IncorrectData) : {string.Join(",", inValidIssueType.Select(c => c.ID))}", string.Empty, new Exception("Job Monitoring - Invalid IssueType"), sentryClient);
                        }

                        var lastScannedID = result.InvalidDataList.Max(c => c.ID);

                        if (lastScannedID > 0)
                        {
                            systemParameterServices.UpdateTimingExecution(lastScannedID.ToString(), CTS_SystemParameters.JobMonitoring_IncorrectData_LastScanID);
                        }
                    }
                } while (isContinue);
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