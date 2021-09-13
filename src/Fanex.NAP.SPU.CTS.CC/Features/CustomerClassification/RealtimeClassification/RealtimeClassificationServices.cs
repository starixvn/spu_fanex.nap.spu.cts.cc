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

        IRealtimeClassificationServices ScanningRealtimeClassification(int batchSize);

        IEnumerable<CustClassificationRealtimeEntity> GetCustomerGeneralCategoryForRunRealtime(IEnumerable<CustClassificationRealtimeEntity> customerHasChanges);

        void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustClassificationRealtimeEntity> custHasChanges, IEnumerable<CustClassificationRealtimeEntity> custClassification);

        IRealtimeClassificationServices RealtimeUpdateLastScanTime(bool isGeneralCategory);

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

        public IRealtimeClassificationServices ScanningRealtimeClassification(int batchSize)
        {
            if (customerClassification != null && customerClassification.Any())
            {
                var custChunks = customerClassification.Chunk(batchSize).ToList();

                foreach (var custHasChange in custChunks)
                {
                    var custClassification = GetCustomerGeneralCategoryForRunRealtime(custHasChange);

                    ClassifyCustomerGeneralCategoryRealtime(custHasChange, custClassification);

                    Thread.Sleep(Constants.Realtime_SleepTimes);
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

        public void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustClassificationRealtimeEntity> custHasChanges, IEnumerable<CustClassificationRealtimeEntity> custClassification)
        {
            if (custHasChanges != null && custHasChanges.Any())
            {
                var custsClassify = custHasChanges.GroupJoin(custClassification,
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

        IEnumerable<CustClassificationRealtimeEntity> IRealtimeClassificationServices.CustomerClassification => customerClassification;
    }
}