namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
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

        IEnumerable<long> GetRealtimeCustomerPlaceNewBet(DateTime fromScannedTime, DateTime toScannedTime);

        void ClearRealtimeCustomerPlaceNewBets(DateTime fromScannedTime, DateTime toScannedTime);

        IClassificationJobMonitorService CheckRealtimeCustomerCategory(IEnumerable<CustomerCategoriesMonitorEntity> custCategory);

        IClassificationJobMonitorService InsertMonitorInvalidCustomerCategory(int batchSize);

        IClassificationJobMonitorService NotifyTelegram(string environment, DateTime scannedTimeFrom, DateTime scannedTimeTo);

        IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(IEnumerable<long> custIds, int batchSize);
    }

    public class ClassificationJobMonitorService : IClassificationJobMonitorService
    {
        private readonly IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess;
        private IEnumerable<CustomerCategoriesMonitorEntity> realtimeNullCategory;
        private IEnumerable<CustomerCategoriesMonitorEntity> realtimeInactiveCategory;
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
            realtimeNullCategory = new List<CustomerCategoriesMonitorEntity>();
            realtimeInactiveCategory = new List<CustomerCategoriesMonitorEntity>();

            return this;
        }

        public IEnumerable<long> GetRealtimeCustomerPlaceNewBet(DateTime fromScannedTime, DateTime toScannedTime)
        {
            return classificationJobMonitorDataAccess.GetRealtimeCustomerPlaceNewBet(fromScannedTime, toScannedTime);
        }

        public void ClearRealtimeCustomerPlaceNewBets(DateTime fromScannedTime, DateTime toScannedTime)
        {
            classificationJobMonitorDataAccess.ClearRealtimeCustomerPlaceNewBets(fromScannedTime, toScannedTime);
        }

        public IClassificationJobMonitorService CheckRealtimeCustomerCategory(IEnumerable<CustomerCategoriesMonitorEntity> custCategory)
        {
            if (custCategory != null && custCategory.Any())
            {
                realtimeInactiveCategory = custCategory.Where(c => c.CurrentCategoryID == (int)Category.NormalInactive
                                                                    || c.CurrentCategoryID == (int)Category.SmartInactive);

                realtimeNullCategory = custCategory.Where(c => c.CurrentCategoryID == null);
            }

            return this;
        }

        public IClassificationJobMonitorService NotifyTelegram(string environment, DateTime scannedTimeFrom, DateTime scannedTimeTo)
        {
            if (realtimeInactiveCategory.Any())
            {
                NotifyTelegram(realtimeInactiveCategory.Select(c => c.CustId).Distinct(), environment, scannedTimeFrom, scannedTimeTo, JobMonitorType.Realtime_NewPlaceBetInactiveCategory);
            }

            if (realtimeNullCategory.Any())
            {
                NotifyTelegram(realtimeNullCategory.Select(c => c.CustId).Distinct(), environment, scannedTimeFrom, scannedTimeTo, JobMonitorType.Realtime_NewPlaceBetNullCategory);
            }

            return this;
        }

        public IClassificationJobMonitorService InsertMonitorInvalidCustomerCategory(int batchSize)
        {
            if (realtimeInactiveCategory.Any())
            {
                var inactiveCategory = realtimeInactiveCategory.Select(c => new IncorrectCustomerCategoriesMonitorEntity { CustId = c.CustId, CategoryID = c.CurrentCategoryID });

                InsertMonitorInvalidCustomerCategory(inactiveCategory, JobMonitorType.Realtime_NewPlaceBetInactiveCategory, batchSize);
            }

            if (realtimeNullCategory.Any())
            {
                var nullCategory = realtimeInactiveCategory.Select(c => new IncorrectCustomerCategoriesMonitorEntity { CustId = c.CustId, CategoryID = c.CurrentCategoryID });

                InsertMonitorInvalidCustomerCategory(nullCategory, JobMonitorType.Realtime_NewPlaceBetNullCategory, batchSize);
            }

            return this;
        }

        public IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(IEnumerable<long> custIds, int batchSize)
        {
            var custCategory = new List<CustomerCategoriesMonitorEntity>();

            if (custIds.Any())
            {
                var custChunks = custIds.Distinct().Chunk(batchSize);

                foreach (var custBatch in custChunks)
                {
                    var custToString = string.Join(',', custBatch);

                    custCategory.AddRange(classificationJobMonitorDataAccess.GetCustomerCategoriesToMonitor(custToString));
                }
            }

            return custCategory;
        }

        private void InsertMonitorInvalidCustomerCategory(IEnumerable<IncorrectCustomerCategoriesMonitorEntity> customerList, JobMonitorType type, int batchSize)
        {
            if (customerList.Any())
            {
                var custChunks = customerList.Distinct().Chunk(batchSize);

                foreach (var custBatch in custChunks)
                {
                    var customerJson = JsonConvert.SerializeObject(custBatch);

                    classificationJobMonitorDataAccess.InsertMonitorIncorrectCustCategory(customerJson, type);
                }
            }
        }

        private static void NotifyTelegram(IEnumerable<long> custIds, string environment, DateTime scannedTimeFrom, DateTime scannedTimeTo, JobMonitorType type)
        {
            if (custIds.Any())
            {
                var message = string.Empty;

                custIds = custIds.Distinct();

                switch (type)
                {
                    case JobMonitorType.Realtime_NewPlaceBetNullCategory:
                        {
                            message = $"Please kindly check the following custid with category NULL - {{Total custid: {custIds.Count()}}} - {string.Join(',', custIds.Take(50))} - " +
                                      $"Scan from  \"{scannedTimeFrom:dd/MM/yyyy HH:mm:ss}\" to  \"{scannedTimeTo:dd/MM/yyyy HH:mm:ss}\"";
                            break;
                        }
                    case JobMonitorType.Realtime_NewPlaceBetInactiveCategory:
                        {
                            message = $"Please kindly check the following custid NOT RE-ACTIVATE - {{Total custid: {custIds.Count()}}} - {string.Join(',', custIds.Take(50))} - " +
                                      $"Scan from \"{scannedTimeFrom:dd/MM/yyyy HH:mm:ss}\" to \"{scannedTimeTo:dd/MM/yyyy HH:mm:ss}\"";
                            break;
                        }
                }

                Utilities.NotifyTelegramMessage(message, "Monitoring - Realtime Classification", environment, environment == "LOCAL" ? Telegram_GroupId_Local : Telegram_JobMonitor_GroupId_Pro);
            }
        }

        #endregion Realtime inactive category monitor
    }
}