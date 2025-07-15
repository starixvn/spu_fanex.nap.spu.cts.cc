namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using System.Linq;

    public interface IRealtimeClassificationBySportServices
    {
        IRealtimeClassificationBySportServices Start();

        IRealtimeClassificationBySportServices GetNewPlaceBetCustomers();

        IRealtimeClassificationBySportServices PreprocessCustomerClassRealtime(int internalBatchSize);        

        IRealtimeClassificationBySportServices CompleteCustomerRealtime();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Datetime Code Smell", "S6566:Prefer using DateTimeOffset instead of DateTime", Justification = "<NotHandle>")]
    public class RealtimeClassificationBySportServices : IRealtimeClassificationBySportServices
    {
        private readonly IRealtimeClassificationBySportDataAccess dataAccess;
        private GetNewPlaceBetCustomerModel customers;


        // private const string ServiceName = "Realtime Classification By Sport Service";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
        public RealtimeClassificationBySportServices(IRealtimeClassificationBySportDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
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
