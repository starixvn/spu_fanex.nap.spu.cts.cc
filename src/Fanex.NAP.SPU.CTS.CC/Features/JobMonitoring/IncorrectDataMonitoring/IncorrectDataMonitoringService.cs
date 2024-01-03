namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IIncorrectDataMonitoringService
    {
        IEnumerable<GetMonitoringEntity> GetIncorrectDataMonitoring();

        void InsertIncorrectDataMonitoring(IEnumerable<InsertMonitoringEntity> incorrectList, JobMonitor_IssueType issueType, int batchSize);

        void InsertIncorrectDataMonitoring(IEnumerable<long> incorrectList, JobMonitor_IssueType issueType, int batchSize);

        void NotifyTelegram(IEnumerable<GetMonitoringEntity> incorrectList, string environment);
    }

    public class IncorrectDataMonitoringService : IIncorrectDataMonitoringService
{
        private readonly IIncorrectDataMonitoringDataAccess incorrectDataMonitoringDataAccess;
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "Incorrect Data Monitoring Job";

        public IncorrectDataMonitoringService(IIncorrectDataMonitoringDataAccess incorrectDataMonitoringDataAccess, ISentryClient sentryClient)
        {
            this.incorrectDataMonitoringDataAccess = incorrectDataMonitoringDataAccess;
            this.sentryClient = sentryClient;
        }

        public IEnumerable<GetMonitoringEntity> GetIncorrectDataMonitoring()
        {
            return incorrectDataMonitoringDataAccess.GetIncorrectDataMonitoring();
        }


        public void InsertIncorrectDataMonitoring(IEnumerable<InsertMonitoringEntity> incorrectList, JobMonitor_IssueType issueType, int batchSize)
        {
            if (incorrectList != null && incorrectList.Any())
            {
                var dataChunks = incorrectList.Chunk(batchSize);

                foreach (var logDataBatch in dataChunks)
                {
                    var dataJson = JsonConvert.SerializeObject(logDataBatch);
                    incorrectDataMonitoringDataAccess.InsertIncorrectDataMonitoring(dataJson, issueType);
                }
            }
        }

        public void InsertIncorrectDataMonitoring(IEnumerable<long> incorrectList, JobMonitor_IssueType issueType, int batchSize)
        {
            if (incorrectList != null && incorrectList.Any())
            {
                var dataChunks = incorrectList.Chunk(batchSize);

                foreach (var logDataBatch in dataChunks)
                {
                    var dataJson = JsonConvert.SerializeObject(logDataBatch);
                    incorrectDataMonitoringDataAccess.InsertIncorrectDataMonitoring(dataJson, issueType);
                }
            }
        }

        public void NotifyTelegram(IEnumerable<GetMonitoringEntity> incorrectList, string environment)
        {
            if (incorrectList == null || !incorrectList.Any())
            {
                return;
            }

            var begin = incorrectList.Min(c => c.CreatedTime).ToString(CONVERT_TO_DATETIME);
            var end = incorrectList.Max(c => c.CreatedTime).ToString(CONVERT_TO_DATETIME);

            string message = $"Please kindly check the following case(s): \n (Scan from  {begin} to {end} )\n";

            foreach (JobMonitor_IssueType val in Enum.GetValues(typeof(JobMonitor_IssueType)))
            {
                message += GenerateMessage(incorrectList, val);
            }

            Utilities.NotifyTelegramMessage(message, "Job Monitoring", environment, environment == "LOCAL" ? Telegram_GroupId_Local : Telegram_JobMonitor_GroupId_Pro);
        }

        private static string GenerateMessage(IEnumerable<GetMonitoringEntity> data, JobMonitor_IssueType issueTypeID)
        {
            var message = string.Empty;

            if (data.Any())
            {
                var custIDs = data.Where(c => c.IssueTypeID == (int)issueTypeID)?.Select(c => c.CustID)?.Distinct();
                var trackingInfo = data.Where(c => c.IssueTypeID == (int)issueTypeID)?.OrderByDescending(c => c.ID).FirstOrDefault();

                switch (issueTypeID)
                {
                    case JobMonitor_IssueType.NewCustomer_NullCategory:
                        {
                            message += $"New Customer with category NULL  - {{Total custid: {custIDs.Count()}}} - {string.Join(',', custIDs.Take(50))}\n\n";
                            break;
                        }
                    case JobMonitor_IssueType.Realtime_NewPlaceBetInactiveCategory:
                        {
                            message += $"Realtime Classification: custid NOT RE-ACTIVATE category - {{Total custid: {custIDs.Count()}}} - {string.Join(',', custIDs.Take(50))}\n\n";
                            break;
                        }
                    case JobMonitor_IssueType.Realtime_NullCategory:
                        {
                            message += $"Realtime Classification: custid with NULL category - {{Total custid: {custIDs.Count()}}} - {string.Join(',', custIDs.Take(50))}\n\n";
                            break;
                        }
                    case JobMonitor_IssueType.NewCustomer_Missing:
                        {
                            message += $"New Customer with not available on CTS - {{Total custid: {custIDs.Count()}}} - {string.Join(',', custIDs.Take(50))}\n\n";
                            break;
                        }
                    case JobMonitor_IssueType.Classification_GeneralPoolStatus:
                    case JobMonitor_IssueType.Classification_BySportPoolStatus:
                        {
                            message += $"{(issueTypeID == JobMonitor_IssueType.Classification_GeneralPoolStatus ? "General" : "BySport")} Classification Pool exceeds {trackingInfo} \n\n";
                            break;
                        }
                    case JobMonitor_IssueType.DCSTransaction_LastLoginProcessStatus:
                        {
                            message += $"Last Login Transaction Process exceeds {trackingInfo} \n\n";
                            break;
                        }
                }
            }

            return message;
        }
    }
}