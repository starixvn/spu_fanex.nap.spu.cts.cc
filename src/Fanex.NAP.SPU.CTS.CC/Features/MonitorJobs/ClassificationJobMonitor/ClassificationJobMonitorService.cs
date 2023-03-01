namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IClassificationJobMonitorService
    {
        IClassificationJobMonitorService Start();

        IEnumerable<long> GetCustomerGeneralCategoryHasChangeForRunRealtime(DateTime fromScannedTime, DateTime toScannedTime);

        IClassificationJobMonitorService CheckRealtimeInactiveCategory(IEnumerable<CustomerCategoriesMonitorEntity> custCategory);

        IClassificationJobMonitorService InsertMonitorInvalidCustomerCategory();

        IClassificationJobMonitorService NotifyTelegram(string environment, DateTime scannedTimeFrom, DateTime scannedTimeTo);

        IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(IEnumerable<long> custIds);

        void InsertMonitorInvalidCustomerCategory(IEnumerable<IncorrectCustomerCategoriesMonitorEntity> customerList, JobMonitorType type);
    }

    public class ClassificationJobMonitorService : IClassificationJobMonitorService
    {
        private readonly IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess;
        private IEnumerable<CustomerCategoriesMonitorEntity> realtimeCustomerPlaceBet;
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "Classification Job Monitor";

        public ClassificationJobMonitorService(IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess, ISentryClient sentryClient)
        {
            this.classificationJobMonitorDataAccess = classificationJobMonitorDataAccess;
            this.sentryClient = sentryClient;
        }

        #region Realtime inactive category monitor

        public IClassificationJobMonitorService Start()
        {
            realtimeCustomerPlaceBet = new List<CustomerCategoriesMonitorEntity>();

            return this;
        }

        public IEnumerable<long> GetCustomerGeneralCategoryHasChangeForRunRealtime(DateTime fromScannedTime, DateTime toScannedTime)
        {
            return classificationJobMonitorDataAccess.GetRealtimeCustomerNewPlaceBet(fromScannedTime, toScannedTime);
        }

        public IClassificationJobMonitorService CheckRealtimeInactiveCategory(IEnumerable<CustomerCategoriesMonitorEntity> custCategory)
        {
            if (custCategory != null && custCategory.Any())
            {
                realtimeCustomerPlaceBet = custCategory.Where(c => c.CurrentCategoryID == (int)Category.NormalInactive
                                                                    || c.CurrentCategoryID == (int)Category.SmartInactive
                                                                    || c.CurrentCategoryID == null);
            }

            return this;
        }

        public IClassificationJobMonitorService NotifyTelegram(string environment, DateTime scannedTimeFrom, DateTime scannedTimeTo)
        {
            if (realtimeCustomerPlaceBet.Any())
            {
                var custIds = realtimeCustomerPlaceBet.Select(c => c.CustId).Distinct();

                var message = $"Please kindly check the following custid NOT RE-ACTIVATE - {{Total custid: {custIds.Count()}}} - {string.Join(',', custIds.Take(50))} \n" +
                              $"Scan from {scannedTimeFrom:dd/MM/yyyy HH:mm:ss} to {scannedTimeTo:dd/MM/yyyy HH:mm:ss}";

                Utilities.NotifyTelegramMessage(message, "Monitoring - Realtime Customer Inactive Category", environment, "0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", environment == "LOCAL" ? TelegramGroupId_Local : TelegramGroupId_Pro);
            }

            return this;
        }


        public IClassificationJobMonitorService InsertMonitorInvalidCustomerCategory()
        {
            if (realtimeCustomerPlaceBet.Any())
            {
                var incorrectCategory = realtimeCustomerPlaceBet.Select(c => new IncorrectCustomerCategoriesMonitorEntity { CustId = c.CustId, CategoryID = c.CurrentCategoryID });
                
                var customerJson = JsonConvert.SerializeObject(incorrectCategory);

                classificationJobMonitorDataAccess.InsertMonitorIncorrectCustCategory(customerJson, JobMonitorType.NotReactiveCategory);

            }
            return this;
        }

        public IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var custToString = string.Join(',', custIds.Distinct());

                return classificationJobMonitorDataAccess.GetCustomerCategoriesToMonitor(custToString);
            }

            return Enumerable.Empty<CustomerCategoriesMonitorEntity>();
        }

        public void InsertMonitorInvalidCustomerCategory(IEnumerable<IncorrectCustomerCategoriesMonitorEntity> customerList, JobMonitorType type)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            classificationJobMonitorDataAccess.InsertMonitorIncorrectCustCategory(customerJson, type);
        }

        #endregion Realtime inactive category monitor
    }
}