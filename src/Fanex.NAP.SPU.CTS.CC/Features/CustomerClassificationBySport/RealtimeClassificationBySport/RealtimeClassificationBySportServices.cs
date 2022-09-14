namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public interface IRealtimeClassificationBySportServices
    {
        IRealtimeClassificationBySportServices Start();

        IRealtimeClassificationBySportServices GetCustomerGeneralCategoryHasChangeForRunRealtime();

        IRealtimeClassificationBySportServices ScanningRealtimeClassification(int externalBatchSize, int internalBatchSize);

        IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerGeneralCategoryForRunRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customerHasChanges);

        void ClassifyCustomerGeneralCategoryRealtime(CustClassificationRealtimeBySportViewEntity classification);

        IRealtimeClassificationBySportServices RealtimeUpdateLastScanTime(bool isGeneralCategory);

        IEnumerable<CustClassificationRealtimeBySportEntity> CustomerClassification { get; }
    }

    public class RealtimeClassificationBySportServices : IRealtimeClassificationBySportServices
    {
        private readonly IRealtimeClassificationBySportDataAccess realtimeClassificationDataAccess;
        private IEnumerable<CustClassificationRealtimeBySportEntity> customerClassification;
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "Realtime Classification Service";

        public RealtimeClassificationBySportServices(IRealtimeClassificationBySportDataAccess realtimeClassificationDataAccess
                                                    , ISentryClient sentryClient)
        {
            this.realtimeClassificationDataAccess = realtimeClassificationDataAccess;
            this.sentryClient = sentryClient;
        }

        public IRealtimeClassificationBySportServices Start()
        {
            customerClassification = new List<CustClassificationRealtimeBySportEntity>();

            return this;
        }

        public IRealtimeClassificationBySportServices GetCustomerGeneralCategoryHasChangeForRunRealtime()
        {
            this.customerClassification = realtimeClassificationDataAccess.GetCustomerGeneralCategoryHasChangeForRunRealtime();

            return this;
        }

        public IRealtimeClassificationBySportServices ScanningRealtimeClassification(int externalBatchSize, int internalBatchSize)
        {
            if (customerClassification != null && customerClassification.Any())
            {
                var custExternalChunks = customerClassification.Chunk(externalBatchSize).ToList();

                var custToClassification = new CustClassificationRealtimeBySportViewEntity
                {
                    CustClassification = new List<CustClassificationRealtimeBySportEntity>(),
                    CustCategories = new List<CustClassificationRealtimeBySportEntity>()
                };

                foreach (var custHasChange in custExternalChunks)
                {
                    var custInternalChunks = custHasChange.Chunk(internalBatchSize).ToList();

                    foreach (var custCategory in custInternalChunks)
                    {
                        custToClassification.CustCategories.AddRange(GetCustomerGeneralCategoryForRunRealtime(custCategory));

                        Thread.Sleep(Constants.Realtime_SleepTimes);
                    }

                    custToClassification.CustClassification.AddRange(custHasChange);

                    ClassifyCustomerGeneralCategoryRealtime(custToClassification);

                    custToClassification = new CustClassificationRealtimeBySportViewEntity
                    {
                        CustClassification = new List<CustClassificationRealtimeBySportEntity>(),
                        CustCategories = new List<CustClassificationRealtimeBySportEntity>()
                    };
                }
            }

            return this;
        }

        public IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerGeneralCategoryForRunRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customerHasChanges)
        {
            if (customerHasChanges != null && customerHasChanges.Any())
            {
                var custSportJson = JsonConvert.SerializeObject(customerHasChanges);

                return realtimeClassificationDataAccess.GetCustomerGeneralCategoryForRunRealtime(custSportJson) ?? new List<CustClassificationRealtimeBySportEntity>();
            }

            return new List<CustClassificationRealtimeBySportEntity>();
        }

        public void ClassifyCustomerGeneralCategoryRealtime(CustClassificationRealtimeBySportViewEntity classification)
        {
            if (classification.CustClassification != null && classification.CustClassification.Any())
            {
                var custsClassify = classification.CustClassification.GroupJoin(classification.CustCategories,
                                                    custsHaschanges => custsHaschanges.CustId,
                                                    custsCategories => custsCategories.CustId,
                                                    (custsHaschanges, custsCategories) => new { custsHaschanges, custsCategories })
                                                    .SelectMany(
                                                        group => group.custsCategories.DefaultIfEmpty(),
                                                        (group, temp) => new CustClassificationRealtimeBySportEntity
                                                        {
                                                            CustId = group.custsHaschanges.CustId,
                                                            SportId = group.custsHaschanges.SportId,
                                                            CategoryId = (temp != null) ? temp.CategoryId : 0
                                                        });

                var finalCustToClassify = custsClassify.Where(c => c.CategoryId != -1);

                if (finalCustToClassify.Any())
                {
                    realtimeClassificationDataAccess.ClassifyCustomerGeneralCategoryRealtime(finalCustToClassify);
                }
            }
        }

        public IRealtimeClassificationBySportServices RealtimeUpdateLastScanTime(bool isGeneralCategory)
        {
            realtimeClassificationDataAccess.RealtimeUpdateLastScanTime(isGeneralCategory);

            return this;
        }

        IEnumerable<CustClassificationRealtimeBySportEntity> IRealtimeClassificationBySportServices.CustomerClassification => customerClassification;
    }
}