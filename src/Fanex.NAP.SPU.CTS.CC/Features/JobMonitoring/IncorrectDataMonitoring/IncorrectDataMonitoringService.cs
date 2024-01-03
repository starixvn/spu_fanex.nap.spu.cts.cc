namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using Sentry;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IIncorrectDataMonitoringService
    {
        IEnumerable<GetMonitoringEntity> GetIncorrectDataMonitoring();

        void InsertIncorrectDataMonitoring(IEnumerable<InsertMonitoringEntity> incorrectData, JobMonitorType issueType);
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


        public void InsertIncorrectDataMonitoring(IEnumerable<InsertMonitoringEntity> incorrectData, JobMonitorType issueType)
        {
            incorrectDataMonitoringDataAccess.InsertIncorrectDataMonitoring(incorrectData, issueType);
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