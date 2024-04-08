namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System;
    using System.Linq;

    public interface ITVSProblemAccountDataAccess
    {
        TVSProblemAccountResultEntity GetTWProblemAccount(DateTime? lastDateTime);
    }

    public class TVSProblemAccountDataAccess : ITVSProblemAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public TVSProblemAccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public TVSProblemAccountResultEntity GetTWProblemAccount(DateTime? lastDateTime)
        {
            var criteria = new TVSProblemAccountGetCriteria
            {
                Datetime = lastDateTime
            };

            var result = repository.Fetch<TVSProblemAccountEntity>(criteria);

            return new TVSProblemAccountResultEntity
            {
                ProblemAccounts = result ?? Enumerable.Empty<TVSProblemAccountEntity>(),
                MaxTimeRequestActive = criteria.MaxTimeRequestActive,
                MissingDataRequestIds = criteria.MissingDataRequestIds
            };
        }
    }
}