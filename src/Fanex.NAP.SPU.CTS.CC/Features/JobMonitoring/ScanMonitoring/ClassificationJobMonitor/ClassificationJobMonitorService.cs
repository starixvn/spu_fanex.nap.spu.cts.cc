namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
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

        IClassificationJobMonitorService FilterInvalidCustomerCategory(IEnumerable<CustomerCategoryMonitorEntity> custCategory);

        IClassificationJobMonitorService InsertInvalidCategory(int batchSize);

        IEnumerable<CustomerCategoryMonitorEntity> GetCustomerCategoryToMonitor(IEnumerable<long> custIds, int batchSize);
    }

    public class ClassificationJobMonitorService : IClassificationJobMonitorService
    {
        private readonly IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess;
        private readonly IIncorrectDataMonitoringService incorrectDataMonitoringService;
        private IEnumerable<CustomerCategoryMonitorEntity> realtimeNullCategory;
        private IEnumerable<CustomerCategoryMonitorEntity> realtimeInactiveCategory;
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "Classification Job Monitor";

        public ClassificationJobMonitorService(
                IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess,
                IIncorrectDataMonitoringService incorrectDataMonitoringService,
                ISentryClient sentryClient)
        {
            this.classificationJobMonitorDataAccess = classificationJobMonitorDataAccess;
            this.incorrectDataMonitoringService = incorrectDataMonitoringService;
            this.sentryClient = sentryClient;
        }

        #region Realtime inactive category monitor

        public IClassificationJobMonitorService Start()
        {
            realtimeNullCategory = new List<CustomerCategoryMonitorEntity>();
            realtimeInactiveCategory = new List<CustomerCategoryMonitorEntity>();

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

        public IClassificationJobMonitorService FilterInvalidCustomerCategory(IEnumerable<CustomerCategoryMonitorEntity> custCategory)
        {
            if (custCategory != null && custCategory.Any())
            {
                realtimeInactiveCategory = custCategory.Where(c => c.TypeID == (int)InvalidCategory.Invalid_Inactive);

                realtimeNullCategory = custCategory.Where(c => c.TypeID == (int)InvalidCategory.Invalid_Null);
            }

            return this;
        }

        public IClassificationJobMonitorService InsertInvalidCategory(int batchSize)
        {
            if (realtimeInactiveCategory.Any())
            {
                var inactiveCategory = realtimeInactiveCategory.Select(c => c.CustID).Distinct();

                incorrectDataMonitoringService.InsertIncorrectDataMonitoring(inactiveCategory, JobMonitor_IssueType.Realtime_NewPlaceBetInactiveCategory, batchSize);
            }

            if (realtimeNullCategory.Any())
            {
                var nullCategory = realtimeNullCategory.Select(c => c.CustID).Distinct();

                incorrectDataMonitoringService.InsertIncorrectDataMonitoring(nullCategory, JobMonitor_IssueType.Realtime_NullCategory, batchSize);
            }

            return this;
        }

        public IEnumerable<CustomerCategoryMonitorEntity> GetCustomerCategoryToMonitor(IEnumerable<long> custIds, int batchSize)
        {
            var custCategory = new List<CustomerCategoryMonitorEntity>();

            if (custIds.Any())
            {
                var custChunks = custIds.Distinct().Chunk(batchSize);

                foreach (var custBatch in custChunks)
                {
                    var custToString = string.Join(',', custBatch);

                    custCategory.AddRange(classificationJobMonitorDataAccess.GetCustomerCategoryToMonitor(custToString, false));
                }
            }

            return custCategory;
        }

        #endregion Realtime inactive category monitor
    }
}