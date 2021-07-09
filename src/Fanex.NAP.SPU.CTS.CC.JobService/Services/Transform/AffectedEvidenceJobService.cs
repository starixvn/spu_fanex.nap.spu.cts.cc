namespace Fanex.NAP.SPU.CTS.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.AffectedEvidence;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAffectedEvidenceJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class AffectedEvidenceJobService : IAffectedEvidenceJobService
    {
        private readonly IAffectedEvidenceService affectedEvidenceService;
        private readonly ISentryClient sentryClient;

        public AffectedEvidenceJobService(ISentryClient sentryClient, IAffectedEvidenceService affectedEvidenceService)
        {
            this.sentryClient = sentryClient;
            this.affectedEvidenceService = affectedEvidenceService;
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                var custIds = affectedEvidenceService.TransformAssociationAffectedEvidence(batchSize);

                if (custIds.Any())
                {
                    // Set BetLimit
                    affectedEvidenceService.UpdateBetLimitByEvidenceRemoving(custIds);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return Task.FromResult(false);
            }
        }
    }
}
