namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
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

        IRealtimeClassificationBySportServices PreprocessCustomerClassRealtime(int internalBatchSize);        

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

        public IRealtimeClassificationBySportServices PreprocessCustomerClassRealtime(int internalBatchSize)
        {
            if (this.customers != null && this.customers.CustomerNewPlaceBets.Any())
            {
                var custInternalChunks = customers.CustomerNewPlaceBets.Chunk(internalBatchSize);
                
                foreach (var cust in custInternalChunks)
                {                    
                    dataAccess.PreprocessCustomerClassRealtime(cust);
                }              
            }

            return this;
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
