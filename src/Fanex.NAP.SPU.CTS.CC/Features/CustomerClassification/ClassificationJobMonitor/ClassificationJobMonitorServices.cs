namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IClassificationJobMonitorServices
    {
        IClassificationJobMonitorServices Start();

        IEnumerable<long> GetCustomerGeneralCategoryHasChangeForRunRealtime(DateTime fromScannedTime, DateTime toScannedTime);

        IClassificationJobMonitorServices CheckRealtimeInactiveCategory(IEnumerable<CustClassificationRealtimeEntity> custCategory);

        IClassificationJobMonitorServices NotifyTelegram(string environment);
    }

    public class ClassificationJobMonitorServices : IClassificationJobMonitorServices
    {
        private readonly IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess;
        private IEnumerable<long> realtimeCustomerPlaceBet;
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "Classification Job Monitor";

        public ClassificationJobMonitorServices(IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess, ISentryClient sentryClient)
        {
            this.classificationJobMonitorDataAccess = classificationJobMonitorDataAccess;
            this.sentryClient = sentryClient;
        }

        public IClassificationJobMonitorServices Start()
        {
            realtimeCustomerPlaceBet = new List<long>();

            return this;
        }

        public IEnumerable<long> GetCustomerGeneralCategoryHasChangeForRunRealtime(DateTime fromScannedTime, DateTime toScannedTime)
        {
            return classificationJobMonitorDataAccess.GetRealtimeCustomerNewPlaceBet(fromScannedTime, toScannedTime);
        }

        public IClassificationJobMonitorServices CheckRealtimeInactiveCategory(IEnumerable<CustClassificationRealtimeEntity> custCategory)
        {
            if (custCategory != null && custCategory.Any())
            {
                realtimeCustomerPlaceBet = custCategory.Where(c => c.CurrentCategoryID == (int)Category.NormalInactive || c.CurrentCategoryID == (int)Category.SmartInactive)
                                                       .Select(c => c.CustId).Distinct();
            }

            return this;
        }

        public IClassificationJobMonitorServices NotifyTelegram(string environment)
        {
            if (realtimeCustomerPlaceBet.Any())
            {
                string message = "";
                string token = "";
                string groupIds = "";

                Utilities.NotifyTelegramMessage(message, "Monitoring - Realtime Customer Inactive Category", environment, token, groupIds);
            }

            return this;
        }
    }
}