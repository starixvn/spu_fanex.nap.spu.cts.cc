namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public interface IRealtimeClassificationServices
    {
        void CheckNewChangesForRealtime();

        IRealtimeClassificationServices Start();

        IRealtimeClassificationServices GetCustomerGeneralCategoryHasChangeForRunRealtime();

        IRealtimeClassificationServices ScanningRealtimeClassification(int externalBatchSize, int internalBatchSize);

        IEnumerable<CustClassificationRealtimeEntity> GetCustomerGeneralCategoryForRunRealtime(IEnumerable<CustClassificationRealtimeEntity> customerHasChanges);

        void ClassifyCustomerGeneralCategoryRealtime(CustClassificationRealtimeViewEntity classification);

        IRealtimeClassificationServices UpdateGeneralRealtimeLastScan();

        IRealtimeClassificationServices UpdateLastTicketAssociationCustomer(int internalBatchSize);

        IEnumerable<CustClassificationRealtimeEntity> CustomerClassification { get; }
    }

    public class RealtimeClassificationServices : IRealtimeClassificationServices
    {
        private readonly IRealtimeClassificationDataAccess realtimeClassificationDataAccess;
        private IEnumerable<CustClassificationRealtimeEntity> customerClassification;
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "Realtime Classification Service";

        public RealtimeClassificationServices(IRealtimeClassificationDataAccess realtimeClassificationDataAccess, ISentryClient sentryClient)
        {
            this.realtimeClassificationDataAccess = realtimeClassificationDataAccess;
            this.sentryClient = sentryClient;
        }

        public IRealtimeClassificationServices Start()
        {
            customerClassification = new List<CustClassificationRealtimeEntity>();

            return this;
        }
        public void CheckNewChangesForRealtime()
        {
            realtimeClassificationDataAccess.CheckNewChangesForRealtime();
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
                                                            CurrentCategoryID = (temp != null) ? temp.CurrentCategoryID : 0
                                                        });

                var finalCustToClassify = custsClassify.Where(c => c.CurrentCategoryID != -1);

                if (finalCustToClassify.Any())
                {
                    realtimeClassificationDataAccess.ClassifyCustomerGeneralCategoryRealtime(finalCustToClassify);
                }
            }
        }

        public IRealtimeClassificationServices UpdateGeneralRealtimeLastScan()
        {
            realtimeClassificationDataAccess.UpdateGeneralRealtimeLastScan(true);

            return this;
        }

        public IRealtimeClassificationServices UpdateLastTicketAssociationCustomer(int internalBatchSize)
        {
            if (this.customerClassification != null && this.customerClassification.Any())
            {
                var custIds = this.customerClassification.Select(x => new { CustID = x.CustId });

                var custIdChunks = custIds.Chunk(internalBatchSize).ToList();

                foreach (var custIdItems in custIdChunks)
                {
                    var custIdList = string.Join(",", custIdItems.Select(x => x.CustID));

                    realtimeClassificationDataAccess.UpdateAssociationCustomerLastTicket(custIdList);
                }
            }

            return this;
        }

        IEnumerable<CustClassificationRealtimeEntity> IRealtimeClassificationServices.CustomerClassification => customerClassification;
    }
}