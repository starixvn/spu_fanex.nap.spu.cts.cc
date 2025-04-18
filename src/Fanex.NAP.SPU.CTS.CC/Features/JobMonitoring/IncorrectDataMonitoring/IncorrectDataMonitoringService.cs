namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring.Models;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IIncorrectDataMonitoringService
    {
        GetNotificationMonitoringEntity GetIncorrectDataMonitoring();

        void InsertIncorrectDataMonitoring(IEnumerable<InsertMonitoringEntity> incorrectList, JobMonitor_IssueType issueType, int batchSize);

        void InsertIncorrectDataMonitoring(IEnumerable<long> incorrectList, JobMonitor_IssueType issueType, int batchSize);

        void SendNotification(IEnumerable<GetInvalidDataMonitoringEntity> incorrectList, string environment, string groupId);
    }

    public class IncorrectDataMonitoringService : IIncorrectDataMonitoringService
    {
        private readonly IIncorrectDataMonitoringDataAccess incorrectDataMonitoringDataAccess;
        private const int MaxItemsToDisplay = 50;

        // private const string ServiceName = "Incorrect Data Monitoring Job";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
        public IncorrectDataMonitoringService(IIncorrectDataMonitoringDataAccess incorrectDataMonitoringDataAccess)
        {
            this.incorrectDataMonitoringDataAccess = incorrectDataMonitoringDataAccess;
        }

        public GetNotificationMonitoringEntity GetIncorrectDataMonitoring()
        {
            return incorrectDataMonitoringDataAccess.GetIncorrectDataMonitoring();
        }

        public void InsertIncorrectDataMonitoring(IEnumerable<InsertMonitoringEntity> incorrectList, JobMonitor_IssueType issueType, int batchSize)
        {
            if (incorrectList?.Any() != true)
            {
                return;
            }

            foreach (var logDataBatch in incorrectList.Chunk(batchSize))
            {
                var dataJson = JsonConvert.SerializeObject(logDataBatch);
                incorrectDataMonitoringDataAccess.InsertIncorrectDataMonitoring(dataJson, issueType);
            }
        }

        public void InsertIncorrectDataMonitoring(IEnumerable<long> incorrectList, JobMonitor_IssueType issueType, int batchSize)
        {
            if (incorrectList?.Any() != true)
            {
                return;
            }

            foreach (var logDataBatch in incorrectList.Chunk(batchSize))
            {
                var incorrectData = logDataBatch.Select(custId => new InsertMonitoringEntity { CustID = custId }).ToList();
                var dataJson = JsonConvert.SerializeObject(incorrectData);
                incorrectDataMonitoringDataAccess.InsertIncorrectDataMonitoring(dataJson, issueType);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1643:Strings should not be concatenated using '+' in a loop", Justification = "<Pending>")]
        public void SendNotification(IEnumerable<GetInvalidDataMonitoringEntity> incorrectList, string environment, string groupId)
        {
            if (incorrectList?.Any() != true)
            {
                return;
            }

            var begin = incorrectList.Min(c => c.CreatedTime).ToString(CONVERT_TO_DATETIME);
            var end = incorrectList.Max(c => c.CreatedTime).ToString(CONVERT_TO_DATETIME);

            string message = $"Please kindly check the following case(s): \r\n<code>(Scanning data from  {begin} to {end})</code>";
            message += "<pre>";

            foreach (JobMonitor_IssueType issueType in Enum.GetValues(typeof(JobMonitor_IssueType)))
            {
                message += GenerateMessage(incorrectList, issueType);
            }
            message += "</pre>";

            Utilities.NotifyChannelMessage(message, "Job Monitoring", environment, groupId);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        private static string GenerateMessage(IEnumerable<GetInvalidDataMonitoringEntity> data, JobMonitor_IssueType issueTypeID)
        {
            if (data?.Any() != true)
            {
                return string.Empty;
            }

            var filteredData = data.Where(c => c.IssueTypeID == (int)issueTypeID).ToList();
            if (!filteredData.Any())
            {
                return string.Empty;
            }

            var rawCustIds = filteredData
                .Select(c => new GetInvalidDataMonitoringModel
                {
                    CustID = c.CustID,
                    IsResolved = c.IsResolved
                })
                .Distinct()
                .ToList();

            var trackingInfo = filteredData.OrderByDescending(c => c.ID).FirstOrDefault();
            var values = filteredData.Select(c => c.Value).Distinct().ToList();

            if (!rawCustIds.Any() && trackingInfo == null)
            {
                return string.Empty;
            }

            var custIDs = rawCustIds.Select(c => c.CustID).Distinct().ToList();

            return GetMessageForIssueType(issueTypeID, custIDs, rawCustIds, trackingInfo, values);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        private static string GetMessageForIssueType(
            JobMonitor_IssueType issueTypeID, 
            IEnumerable<int?> custIDs, 
            IEnumerable<GetInvalidDataMonitoringModel> rawCustIds, 
            GetInvalidDataMonitoringEntity trackingInfo, 
            IEnumerable<long?> values)
        {
            switch (issueTypeID)
            {
                case JobMonitor_IssueType.NewCustomer_NullCategory:
                case JobMonitor_IssueType.Realtime_NullCategory:
                    return BuildMessageWithContent(custIDs, "🆘 NULL customer category");

                case JobMonitor_IssueType.NewCustomer_Missing:
                    return BuildMessageWithCorrectAndIncorrectContent(
                        rawCustIds, 
                        "✅ Inserted missing customer", 
                        "🆘 Missing customer");

                case JobMonitor_IssueType.ReactivatedCustomer_Missing:
                    return BuildMessageWithCorrectAndIncorrectContent(
                        rawCustIds, 
                        "✅ Inserted missing reactivated-customer", 
                        "🆘 Missing reactivated-customer");

                case JobMonitor_IssueType.NewCustomer_TotalUnresolvedMissingCustomers:
                    return $"\r\n🆘 Total missing customers so far  {{Total custid: {trackingInfo?.Value}}} \r\n";

                case JobMonitor_IssueType.NewCustomer_IncorrectCustInfo:
                    return BuildMessageWithContent(custIDs, "🆘 Incorrect customer info");

                case JobMonitor_IssueType.Realtime_NewPlaceBetInactiveCategory:
                    return BuildMessageWithContent(custIDs, "🆘 Incorrect customer category");

                case JobMonitor_IssueType.Classification_ExcessiveGeneralPool:
                case JobMonitor_IssueType.Classification_ExcessiveBySportPool:
                    {
                        var poolType = issueTypeID == JobMonitor_IssueType.Classification_ExcessiveGeneralPool ? "General" : "BySport";
                        return $"\r\n⚠️ {poolType} Classification Pool exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";
                    }

                case JobMonitor_IssueType.Classification_ExcessiveGerneralNormalInsert:
                case JobMonitor_IssueType.Classification_ExcessiveBySportNormalInsert:
                    {
                        var insertType = issueTypeID == JobMonitor_IssueType.Classification_ExcessiveGerneralNormalInsert ? "General" : "BySport";
                        return $"\r\n⚠️ {insertType} Normal Insert exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";
                    }

                case JobMonitor_IssueType.Realtime_ExcessiveGerneralChanges:
                case JobMonitor_IssueType.Realtime_ExcessiveBySportChanges:
                    {
                        var changesType = issueTypeID == JobMonitor_IssueType.Realtime_ExcessiveGerneralChanges ? "General" : "BySport";
                        return $"\r\n⚠️ {changesType} Realtime Check-Changes exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";
                    }

                case JobMonitor_IssueType.DCSTransaction_LastLoginProcessStatus:
                    return $"\r\n⚠️ Last Login Transaction Process exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";

                case JobMonitor_IssueType.Classification_MissingTVSProblemAccount:
                    return $"\r\n🆘 Missing TVS Problem Account {{Total RequestId: {values.Count()}}} {string.Join(',', values.Take(MaxItemsToDisplay))}\r\n";

                case JobMonitor_IssueType.LatencyCustomer_Missing:
                    return $"\r\n🆘 Missing Latency Account exceeds {trackingInfo?.Value} records\r\n";

                default:
                    return string.Empty;
            }
        }

        private static string BuildMessageWithContent(IEnumerable<int?> custIDs, string content)
        {
            return $"\r\n{content}: {{Total custid: {custIDs.Count()}}} {string.Join(',', custIDs.Take(MaxItemsToDisplay))}\r\n";
        }

        private static string BuildMessageWithCorrectAndIncorrectContent(
            IEnumerable<GetInvalidDataMonitoringModel> rawCustIds, 
            string correctMessage, 
            string uncorrectMessage)
        {
            var message = string.Empty;

            var corrected = rawCustIds.Where(c => c.IsResolved).Select(c => c.CustID).Distinct();
            var uncorrected = rawCustIds.Where(c => !c.IsResolved).Select(c => c.CustID).Distinct();
            
            if (corrected.Any())
            {
                message += $"\r\n{correctMessage} {{Total custid: {corrected.Count()}}} {string.Join(',', corrected.Take(MaxItemsToDisplay))}\r\n";
            }
            
            if (uncorrected.Any())
            {
                message += $"\r\n{uncorrectMessage} {{Total custid: {uncorrected.Count()}}} {string.Join(',', uncorrected.Take(MaxItemsToDisplay))}\r\n";
            }

            return message;
        }
    }
}