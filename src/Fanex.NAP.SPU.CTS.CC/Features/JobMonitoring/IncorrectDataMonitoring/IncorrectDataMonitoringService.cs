namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
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
                    var incorrectData = new List<InsertMonitoringEntity>();

                    logDataBatch.ToList().ForEach(c =>
                    {
                        incorrectData.Add(new InsertMonitoringEntity { CustID = c });
                    });

                    var dataJson = JsonConvert.SerializeObject(incorrectData);
                    incorrectDataMonitoringDataAccess.InsertIncorrectDataMonitoring(dataJson, issueType);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1643:Strings should not be concatenated using '+' in a loop", Justification = "<Pending>")]
        public void SendNotification(IEnumerable<GetInvalidDataMonitoringEntity> incorrectList, string environment, string groupId)
        {
            if (incorrectList == null || !incorrectList.Any())
            {
                return;
            }

            var begin = incorrectList.Min(c => c.CreatedTime).ToString(CONVERT_TO_DATETIME);
            var end = incorrectList.Max(c => c.CreatedTime).ToString(CONVERT_TO_DATETIME);

            string message = $"Please kindly check the following case(s): \r\n<code>(Scanning data from  {begin} to {end})</code>";
            message += "<pre>";

            foreach (JobMonitor_IssueType val in Enum.GetValues(typeof(JobMonitor_IssueType)))
            {
                message += GenerateMessage(incorrectList, val);
            }
            message += "</pre>";

            Utilities.NotifyChannelMessage(message, "Job Monitoring", environment, groupId);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        private static string GenerateMessage(IEnumerable<GetInvalidDataMonitoringEntity> data, JobMonitor_IssueType issueTypeID)
        {
            var message = string.Empty;

            if (data.Any())
            {
                var rawCustIds = data.Where(c => c.IssueTypeID == (int)issueTypeID)?.Select(c => new { c.CustID, c.IsResolved} )?.Distinct();
                var trackingInfo = data.Where(c => c.IssueTypeID == (int)issueTypeID)?.OrderByDescending(c => c.ID).FirstOrDefault();
                var values = data.Where(c => c.IssueTypeID == (int)issueTypeID)?.Select(c => c.Value)?.Distinct();

                if (rawCustIds.Any() || trackingInfo != null)
                {
                    var custIDs = rawCustIds.Select(c => c.CustID).Distinct();

                    switch (issueTypeID)
                    {
                        case JobMonitor_IssueType.NewCustomer_NullCategory:
                        case JobMonitor_IssueType.Realtime_NullCategory:
                            {
                                message += $"\r\n🆘 NULL customer category: {{Total custid: {custIDs.Count()}}} {string.Join(',', custIDs.Take(50))}\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.NewCustomer_Missing:
                            {
                                var corrected = rawCustIds.Where(c => c.IsResolved).Select(c=>c.CustID).Distinct();
                                var uncorrected = rawCustIds.Where(c => !c.IsResolved).Select(c => c.CustID).Distinct();
                                if(corrected.Any()) message += $"\r\n✅ Inserted missing customer {{Total custid: {corrected.Count()}}} {string.Join(',', corrected.Take(50))}\r\n";
                                if(uncorrected.Any()) message += $"\r\n🆘 Missing customer {{Total custid: {uncorrected.Count()}}} {string.Join(',', uncorrected.Take(50))}\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.ReactivatedCustomer_Missing:
                            {
                                var corrected = rawCustIds.Where(c => c.IsResolved).Select(c => c.CustID).Distinct();
                                var uncorrected = rawCustIds.Where(c => !c.IsResolved).Select(c => c.CustID).Distinct();
                                if (corrected.Any()) message += $"\r\n✅ Inserted missing reactivated-customer {{Total custid: {corrected.Count()}}} {string.Join(',', corrected.Take(50))}\r\n";
                                if (uncorrected.Any()) message += $"\r\n🆘 Missing reactivated-customer {{Total custid: {uncorrected.Count()}}} {string.Join(',', uncorrected.Take(50))}\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.NewCustomer_TotalUnresolvedMissingCustomers:
                            {
                                message += $"\r\n🆘 Total missing customers so far  {{Total custid: {trackingInfo.Value}}} \r\n";
                                break;
                            }
                        case JobMonitor_IssueType.NewCustomer_IncorrectCustInfo:
                            {
                                message += $"\r\n🆘 Incorrect customer info {{Total custid: {custIDs.Count()}}} {string.Join(',', custIDs.Take(50))}\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.Realtime_NewPlaceBetInactiveCategory:
                            {
                                message += $"\r\n🆘 Incorrect customer category {{Total custid: {custIDs.Count()}}} {string.Join(',', custIDs.Take(50))} \r\n";
                                break;
                            }
                        case JobMonitor_IssueType.Classification_ExcessiveGeneralPool:
                        case JobMonitor_IssueType.Classification_ExcessiveBySportPool:
                            {
                                var issueTypeConvertToString = issueTypeID == JobMonitor_IssueType.Classification_ExcessiveGeneralPool ? "General" : "BySport";
                                message += $"\r\n⚠️ {issueTypeConvertToString} Classification Pool exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.Classification_ExcessiveGerneralNormalInsert:
                        case JobMonitor_IssueType.Classification_ExcessiveBySportNormalInsert:
                            {
                                var issueTypeConvertToString = issueTypeID == JobMonitor_IssueType.Classification_ExcessiveGerneralNormalInsert ? "General" : "BySport";
                                message += $"\r\n⚠️ {issueTypeConvertToString} Normal Insert exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.Realtime_ExcessiveGerneralChanges:
                        case JobMonitor_IssueType.Realtime_ExcessiveBySportChanges:
                            {
                                var issueTypeConvertToString = issueTypeID == JobMonitor_IssueType.Realtime_ExcessiveGerneralChanges ? "General" : "BySport";
                                message += $"\r\n⚠️ {issueTypeConvertToString} Realtime Check-Changes exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.DCSTransaction_LastLoginProcessStatus:
                            {
                                message += $"\r\n⚠️ Last Login Transaction Process exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.Classification_MissingTVSProblemAccount:
                            {
                                message += $"\r\n🆘 Missing TVS Problem Account {{Total RequestId: {values.Count()}}} {string.Join(',', values.Take(50))}\r\n";
                                break;
                            }
                        case JobMonitor_IssueType.LatencyCustomer_Missing:
                            {
                                message += $"\r\n🆘 Missing Latency Account exceeds {trackingInfo?.Value} records ({trackingInfo?.TrackingInfo})\r\n";
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }

            return message;
        }
    }
}