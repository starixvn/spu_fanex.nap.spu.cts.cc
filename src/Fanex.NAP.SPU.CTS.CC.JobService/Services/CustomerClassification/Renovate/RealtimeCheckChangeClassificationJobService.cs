namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRealtimeCheckChangeClassificationJobService
    {
        Task<bool> Execute();
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

        public Task<bool> Execute()
        {
            try
            {
                var checkChangeModel = realtimeCheckChangeServices.CheckChange();

                if (checkChangeModel.CustInfos != null && checkChangeModel.CustInfos.Any())
                {
                    realtimeCheckChangeServices.UpdateLastTransDate(checkChangeModel.CustInfos);                    
                }

                realtimeCheckChangeServices.UpdateNextScanCheckChange(checkChangeModel.NextScannedDateTime);
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
