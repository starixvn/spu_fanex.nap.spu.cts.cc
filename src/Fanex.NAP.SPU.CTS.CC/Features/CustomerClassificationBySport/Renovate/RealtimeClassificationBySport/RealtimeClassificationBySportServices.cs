namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.RealtimeClassificationBySport
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

        IRealtimeClassificationBySportServices GetNewPlaceBetCustomers();

        IRealtimeClassificationBySportServices PreprocessCustomerClassRealtime(int externalBatchSize, int internalBatchSize);        

        IRealtimeClassificationBySportServices CompleteCustomerRealtime();
    }

    public class RealtimeClassificationBySportServices : IRealtimeClassificationBySportServices
    {
        private readonly IRealtimeClassificationBySportDataAccess dataAccess;
        private GetNewPlaceBetCustomerModel customers;
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "Realtime Classification By Sport Service";

        public RealtimeClassificationBySportServices(IRealtimeClassificationBySportDataAccess dataAccess
                                                    , ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public IRealtimeClassificationBySportServices Start()
        {
            customers = new GetNewPlaceBetCustomerModel();

            return this;
        }

        public IRealtimeClassificationBySportServices GetNewPlaceBetCustomers()
        {
            this.customers = dataAccess.GetNewPlaceBetCustomers();

            return this;
        }       

        public IRealtimeClassificationBySportServices PreprocessCustomerClassRealtime(int externalBatchSize, int internalBatchSize)
        {
            if (this.customers != null && this.customers.CustomerNewPlaceBets.Any())
            {
                var custExternalChunks = customers.CustomerNewPlaceBets.Chunk(externalBatchSize).ToList();
                var scanList = new List<CustClassificationRealtimeBySportEntity>();

                foreach (var custHasChange in custExternalChunks)
                {
                    var custInternalChunks = custHasChange.Chunk(internalBatchSize).ToList();

                    foreach (var custCategory in custInternalChunks)
                    {
                        scanList.AddRange(GetCustomerCategory(custCategory));
                    }
                }

                if (scanList.Any())
                {
                    dataAccess.PreprocessCustomerClassRealtime(scanList);

                    scanList.Clear();
                }
            }

            return this;
        }

        public IEnumerable<CustClassificationRealtimeBySportEntity> GetCustomerCategory(IEnumerable<CustClassificationRealtimeBySportEntity> customerHasChanges)
        {
            if (customerHasChanges != null && customerHasChanges.Any())
            {
                var custSportJson = JsonConvert.SerializeObject(customerHasChanges);

                return dataAccess.GetCustomerCategory(custSportJson);
            }

            return Enumerable.Empty<CustClassificationRealtimeBySportEntity>();
        }

        public IRealtimeClassificationBySportServices CompleteCustomerRealtime()
        {
            if (this.customers != null && this.customers.ScannedMaxId > 0)
            {
                this.dataAccess.CompleteCustomerRealtime(this.customers.ScannedMaxId);
            }

            return this;
        }
    }
}
