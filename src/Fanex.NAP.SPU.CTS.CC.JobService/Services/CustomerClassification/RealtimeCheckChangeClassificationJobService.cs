namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;    
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

                realtimeCheckChangeServices.UpdateLastTransDate(realtimeChanges.CustInfos, batchSize)                
                    .UpdateLastTransDateAgency(realtimeChanges.CustInfosAgency, batchSize)
                    .UpdateNextScanCheckChange(realtimeChanges);
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
