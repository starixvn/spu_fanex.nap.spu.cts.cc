namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification
{
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRealtimeCheckChangeClassificationService
    {
        RealtimeCheckChangeModel CheckChange();

        void UpdateLastTransDate(IEnumerable<RealtimeCheckChangeEntities> CustInfos);

        void UpdateNextScanCheckChange(RealtimeCheckChangeModel nextScanned);

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

        public void UpdateLastTransDate(IEnumerable<RealtimeCheckChangeEntities> CustInfos)
        {
            this.dataAccess.UpdateLastTransDate(JsonConvert.SerializeObject(CustInfos));
        }

        public void UpdateNextScanCheckChange(RealtimeCheckChangeModel nextScanned)
        {
            this.dataAccess.UpdateNextScanCheckChange(nextScanned);
        }
    }
}
