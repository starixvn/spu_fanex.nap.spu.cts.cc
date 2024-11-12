namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public interface IRealtimeCheckChangeClassificationService
    {
        RealtimeCheckChangeModel CheckChange();

        IRealtimeCheckChangeClassificationService UpdateLastTransDate(IEnumerable<RealtimeCheckChangeEntities> custInfos, int batchSize);        

        IRealtimeCheckChangeClassificationService UpdateNextScanCheckChange(RealtimeCheckChangeModel nextScanned);

    }

    public class RealtimeCheckChangeClassificationService : IRealtimeCheckChangeClassificationService
    {
        private readonly IRealtimeCheckChangeClassificationDataAccess dataAccess;

        public RealtimeCheckChangeClassificationService(IRealtimeCheckChangeClassificationDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public RealtimeCheckChangeModel CheckChange()
        {
            return this.dataAccess.CheckChange();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
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

        public IRealtimeCheckChangeClassificationService UpdateNextScanCheckChange(RealtimeCheckChangeModel nextScanned)
        {
            this.dataAccess.UpdateNextScanCheckChange(nextScanned);

            return this;
        }
    }
}
