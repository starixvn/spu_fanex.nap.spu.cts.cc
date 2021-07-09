namespace Fanex.NAP.SPU.CTS.Features.AffectedEvidence
{
    using Sentry;
    using System.Collections.Generic;

    public interface IAffectedEvidenceService
    {
        IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize);

        void UpdateBetLimitByEvidenceRemoving(IEnumerable<int> custIds);
    }

    public class AffectedEvidenceService : IAffectedEvidenceService
    {
        private readonly IAffectedEvidenceDataAccess dataAccess;

        public AffectedEvidenceService(IAffectedEvidenceDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
        }

        public IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize)
        {
            return dataAccess.TransformAssociationAffectedEvidence(batchSize);
        }

        public void UpdateBetLimitByEvidenceRemoving(IEnumerable<int> custIds)
        {
            dataAccess.UpdateBetLimitByEvidenceRemoving(custIds);
        }
    }
}
