namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IClassificationJobMonitorService
    {
        #region Monitor Realtime Category

        IClassificationJobMonitorService Start();

        IEnumerable<long> GetRealtimeCustomerPlaceNewBet(DateTime fromScannedTime, DateTime toScannedTime);

        void ClearRealtimeCustomerPlaceNewBets(DateTime fromScannedTime, DateTime toScannedTime);

        IClassificationJobMonitorService FilterInvalidCustomerCategory(IEnumerable<CustomerCategoryMonitorEntity> custCategory);

        IClassificationJobMonitorService InsertInvalidCategory(int batchSize);

        IEnumerable<CustomerCategoryMonitorEntity> GetCustomerCategoryToMonitor(IEnumerable<long> custIds, int batchSize);

        #endregion Monitor Realtime Category

        #region Monitor Normal Classification Pool

        NormalClassificationPoolMonitorEntity GetNormalClassificationPoolMonitor();

        void NotifyExcessiveClassificationPool(long excessiveNumber, bool isGeneralPool, int batchSize, int thresHold);

        #endregion Monitor Normal Classification Pool
    }

    public class ClassificationJobMonitorService : IClassificationJobMonitorService
    {
        private readonly IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess;
        private readonly IIncorrectDataMonitoringService incorrectDataMonitoringService;
        private IEnumerable<CustomerCategoryMonitorEntity> realtimeNullCategory;
        private IEnumerable<CustomerCategoryMonitorEntity> realtimeInactiveCategory;


        // private const string ServiceName = "Classification Job Monitor";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
        public ClassificationJobMonitorService(
                IClassificationJobMonitorDataAccess classificationJobMonitorDataAccess,
                IIncorrectDataMonitoringService incorrectDataMonitoringService)
        {
            this.classificationJobMonitorDataAccess = classificationJobMonitorDataAccess;
            this.incorrectDataMonitoringService = incorrectDataMonitoringService;
        }

        #region Monitor Realtime Category

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

        #endregion Monitor Realtime Category

        #region Monitor Normal Classification Pool

        public NormalClassificationPoolMonitorEntity GetNormalClassificationPoolMonitor()
        {
            return classificationJobMonitorDataAccess.GetNormalClassificationPoolMonitor();
        }

        public void NotifyExcessiveClassificationPool(long excessiveNumber, bool isGeneralPool, int batchSize, int thresHold)
        {
            if (excessiveNumber > 0)
            {
                var data = new List<InsertMonitoringEntity> { new InsertMonitoringEntity { Value = excessiveNumber, TrackingInfo = $"threshold = {thresHold}" } };
                incorrectDataMonitoringService
                    .InsertIncorrectDataMonitoring(data, isGeneralPool ? JobMonitor_IssueType.Classification_GeneralPoolStatus : JobMonitor_IssueType.Classification_BySportPoolStatus, batchSize);
            }
        }

        #endregion Monitor Normal Classification Pool
    }
}