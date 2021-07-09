namespace Fanex.NAP.SPU.CTS.Features.AffectedEvidence
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.AffectedEvidence.Criteria;
    using Fanex.NAP.SPU.CTS.Features.UserPermission.Criteria;
    using System.Collections.Generic;

    public interface IAffectedEvidenceDataAccess
    {
        IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize);

        void UpdateBetLimitByEvidenceRemoving(IEnumerable<int> custIds);
    }

    public class AffectedEvidenceDataAccess : IAffectedEvidenceDataAccess
    {
        readonly IDynamicRepository repository;

        public AffectedEvidenceDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<int> TransformAssociationAffectedEvidence(int batchSize)
        {
            var criteria = new TransformAssociationAffectedEvidenceCriteria
            {
                BatchSize = batchSize
            };

            return repository.Fetch<int>(criteria);
        }

        public void UpdateBetLimitByEvidenceRemoving(IEnumerable<int> custIds)
        {
            var criteria = new UpdateBetLimitByEvidenceRemovingCriteria
            {
                CustIDList = string.Join(",", custIds)
            };

            repository.Execute(criteria);
        }
    }
}
