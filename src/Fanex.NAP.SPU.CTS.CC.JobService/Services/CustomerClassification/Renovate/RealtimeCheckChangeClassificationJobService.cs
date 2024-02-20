namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRealtimeCheckChangeClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class RealtimeCheckChangeClassificationJobService : IRealtimeCheckChangeClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IRealtimeCheckChangeClassificationService realtimeCheckChangeServices;

        public RealtimeCheckChangeClassificationJobService(ISentryClient sentryClient,
            IRealtimeCheckChangeClassificationService realtimeServices)
        {
            this.sentryClient = sentryClient;
            this.realtimeCheckChangeServices = realtimeServices;
            this.serviceName = "RealtimeCheckChangeClassification";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                var realtimeChanges = realtimeCheckChangeServices.CheckChange();

                if (realtimeChanges.CustInfos != null && realtimeChanges.CustInfos.Any())
                {
                    var batchChunks = realtimeChanges.CustInfos.Chunk(batchSize);

                    foreach (var transDateList in batchChunks)
                    {
                        realtimeCheckChangeServices.UpdateLastTransDate(transDateList);

                        Thread.Sleep(10);
                    }
                }

                realtimeCheckChangeServices.UpdateNextScanCheckChange(realtimeChanges);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
