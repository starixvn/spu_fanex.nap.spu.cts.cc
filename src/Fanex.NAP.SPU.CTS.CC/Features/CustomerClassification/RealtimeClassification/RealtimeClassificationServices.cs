namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public interface IRealtimeClassificationServices
    {
        IRealtimeClassificationServices Start();

        IRealtimeClassificationServices GetCustomerGeneralCategoryHasChangeForRunRealtime();

        IRealtimeClassificationServices ScanningRealtimeClassification(int externalBatchSize, int internalBatchSize);

        IEnumerable<CustClassificationRealtimeEntity> GetCustomerGeneralCategoryForRunRealtime(IEnumerable<CustClassificationRealtimeEntity> customerHasChanges);

        void ClassifyCustomerGeneralCategoryRealtime(CustClassificationRealtimeViewEntity classification);

        IRealtimeClassificationServices RealtimeUpdateLastScanTime(bool isGeneralCategory);

        IRealtimeClassificationServices UpdateLastTicketAssociationCustomer();

        IEnumerable<CustClassificationRealtimeEntity> CustomerClassification { get; }
    }

    public class RealtimeClassificationServices : IRealtimeClassificationServices
    {
        private readonly IRealtimeClassificationDataAccess realtimeClassificationDataAccess;
        private IEnumerable<CustClassificationRealtimeEntity> customerClassification;

        public RealtimeClassificationServices(IRealtimeClassificationDataAccess realtimeClassificationDataAccess)
        {
            this.realtimeClassificationDataAccess = realtimeClassificationDataAccess;
        }

        public IRealtimeClassificationServices Start()
        {
            customerClassification = new List<CustClassificationRealtimeEntity>();

            return this;
        }

        public IRealtimeClassificationServices GetCustomerGeneralCategoryHasChangeForRunRealtime()
        {
            this.customerClassification = realtimeClassificationDataAccess.GetCustomerGeneralCategoryHasChangeForRunRealtime();

            return this;
        }

        public IRealtimeClassificationServices ScanningRealtimeClassification(int externalBatchSize, int internalBatchSize)
        {
            if (customerClassification != null && customerClassification.Any())
            {
                var custExternalChunks = customerClassification.Chunk(externalBatchSize).ToList();

                var custToClassification = new CustClassificationRealtimeViewEntity
                {
                    CustClassification = new List<CustClassificationRealtimeEntity>(),
                    CustCategories = new List<CustClassificationRealtimeEntity>()
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

                    custToClassification = new CustClassificationRealtimeViewEntity
                    {
                        CustClassification = new List<CustClassificationRealtimeEntity>(),
                        CustCategories = new List<CustClassificationRealtimeEntity>()
                    };
                }
            }

            return this;
        }

        public IEnumerable<CustClassificationRealtimeEntity> GetCustomerGeneralCategoryForRunRealtime(IEnumerable<CustClassificationRealtimeEntity> customerHasChanges)
        {
            if (customerHasChanges != null && customerHasChanges.Any())
            {
                var custSportJson = JsonConvert.SerializeObject(customerHasChanges);

                return realtimeClassificationDataAccess.GetCustomerGeneralCategoryForRunRealtime(custSportJson) ?? new List<CustClassificationRealtimeEntity>();
            }

            return new List<CustClassificationRealtimeEntity>();
        }

        public void ClassifyCustomerGeneralCategoryRealtime(CustClassificationRealtimeViewEntity classification)
        {
            if (classification.CustClassification != null && classification.CustClassification.Any())
            {
                var custsClassify = classification.CustClassification.GroupJoin(classification.CustCategories,
                                                    custsHaschanges => custsHaschanges.CustId,
                                                    custsCategories => custsCategories.CustId,
                                                    (custsHaschanges, custsCategories) => new { custsHaschanges, custsCategories })
                                                    .SelectMany(
                                                        group => group.custsCategories.DefaultIfEmpty(),
                                                        (group, temp) => new CustClassificationRealtimeEntity
                                                        {
                                                            CustId = group.custsHaschanges.CustId,
                                                            SportGroupId = group.custsHaschanges.SportGroupId,
                                                            CategoryId = (temp != null) ? temp.CategoryId : 0
                                                        });

                realtimeClassificationDataAccess.ClassifyCustomerGeneralCategoryRealtime(custsClassify);
            }
        }

        public IRealtimeClassificationServices RealtimeUpdateLastScanTime(bool isGeneralCategory)
        {
            realtimeClassificationDataAccess.RealtimeUpdateLastScanTime(isGeneralCategory);

            return this;
        }

        public IRealtimeClassificationServices UpdateLastTicketAssociationCustomer()
        {

            return this;
        }

        IEnumerable<CustClassificationRealtimeEntity> IRealtimeClassificationServices.CustomerClassification => customerClassification;
    }
}