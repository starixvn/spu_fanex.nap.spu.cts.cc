namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;  
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public interface IRealtimeCheckChangeClassificationService
    {
        RealtimeCheckChangeModel CheckChange();

        IRealtimeCheckChangeClassificationService UpdateLastTransDate(IEnumerable<RealtimeCheckChangeEntities> custInfos, int batchSize);

        IRealtimeCheckChangeClassificationService UpdateLastTransDateAgency(IEnumerable<RealtimeCheckChangeAgencyEntities> custInfosAgency, int batchSize);

        IRealtimeCheckChangeClassificationService UpdateNextScanCheckChange(RealtimeCheckChangeModel nextScanned);

    }

    public class RealtimeCheckChangeClassificationService : IRealtimeCheckChangeClassificationService
    {
        private readonly IRealtimeCheckChangeClassificationDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public RealtimeCheckChangeClassificationService(IRealtimeCheckChangeClassificationDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public RealtimeCheckChangeModel CheckChange()
        {
            return this.dataAccess.CheckChange();
        }

        public IRealtimeCheckChangeClassificationService UpdateLastTransDate(IEnumerable<RealtimeCheckChangeEntities> custInfos, int batchSize)
        {
            if (custInfos != null && custInfos.Any())
            {
                var batchChunks = custInfos.Chunk(batchSize);

                foreach (var custTranDates in batchChunks)
                {
                    this.dataAccess.UpdateLastTransDate(JsonConvert.SerializeObject(custTranDates));

                    Thread.Sleep(10);
                }
            }

            return this;
        }

        public IRealtimeCheckChangeClassificationService UpdateLastTransDateAgency(IEnumerable<RealtimeCheckChangeAgencyEntities> custInfosAgency, int batchSize)
        {
            if (custInfosAgency != null && custInfosAgency.Any())
            {
                var batchChunks = custInfosAgency.Chunk(batchSize);

                foreach (var custTranDatesAgency in batchChunks)
                {
                    this.dataAccess.UpdateLastTransDateAgency(JsonConvert.SerializeObject(custTranDatesAgency));

                    Thread.Sleep(10);
                }
            }

            return this;
        }

        public IRealtimeCheckChangeClassificationService UpdateNextScanCheckChange(RealtimeCheckChangeModel nextScanned)
        {
            this.dataAccess.UpdateNextScanCheckChange(nextScanned);

            return this;
        }
    }
}
