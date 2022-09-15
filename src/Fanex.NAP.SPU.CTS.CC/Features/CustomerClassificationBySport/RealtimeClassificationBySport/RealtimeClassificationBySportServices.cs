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

        IRealtimeClassificationBySportServices GetCustomerGeneralCategoryHasChange();

        IRealtimeClassificationBySportServices ScanningRealtimeClassification(int externalBatchSize, int internalBatchSize);

        IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerBySportForRunRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customerHasChanges);

        IRealtimeClassificationBySportServices RealtimeUpdateLastScanTime();

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

        public IRealtimeClassificationBySportServices GetCustomerGeneralCategoryHasChange()
        {
            this.customerClassification = realtimeClassificationDataAccess.GetCustomerGeneralCategoryHasChangeForRunRealtime();

            return this;
        }

        public IRealtimeClassificationBySportServices ScanningRealtimeClassification(int externalBatchSize, int internalBatchSize)
        {
            if (customerClassification != null && customerClassification.Any())
            {
                var custExternalChunks = customerClassification.Chunk(externalBatchSize).ToList();
                var scanList = new List<CustClassificationRealtimeBySportEntity>();

                foreach (var custHasChange in custExternalChunks)
                {
                    var custInternalChunks = custHasChange.Chunk(internalBatchSize).ToList();

                    foreach (var custCategory in custInternalChunks)
                    {
                        scanList.AddRange(GetCustomerBySportForRunRealtime(custCategory));

                        Thread.Sleep(Constants.Realtime_SleepTimes);
                    }

                    if (scanList.Any())
                    {
                        realtimeClassificationDataAccess.ClassifyCustomerBySportRealtime(scanList);
                    }
                }
            }

            return this;
        }

        public IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerBySportForRunRealtime(IEnumerable<CustClassificationRealtimeBySportEntity> customerHasChanges)
        {
            if (customerHasChanges != null && customerHasChanges.Any())
            {
                var custSportJson = JsonConvert.SerializeObject(customerHasChanges);

                return realtimeClassificationDataAccess.GetCustomerBySportForRunRealtime(custSportJson) ?? new List<CustClassificationRealtimeBySportEntity>();
            }

            return Enumerable.Empty<CustClassificationRealtimeBySportEntity>();
        }

        public IRealtimeClassificationBySportServices RealtimeUpdateLastScanTime()
        {
            realtimeClassificationDataAccess.RealtimeUpdateLastScanTime();

            return this;
        }

        IEnumerable<CustClassificationRealtimeBySportEntity> IRealtimeClassificationBySportServices.CustomerClassification => customerClassification;
    }
}