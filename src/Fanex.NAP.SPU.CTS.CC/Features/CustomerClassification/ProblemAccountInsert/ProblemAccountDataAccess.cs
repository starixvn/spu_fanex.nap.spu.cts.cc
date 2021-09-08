namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;

    public interface IProblemAccountDataAccess
    {
        IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime);

        IEnumerable<ProblemAccountsEntity> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime);

        InsertCategoryResultEntity InsertProblemAccount(string problemAccountJson, bool isRescan);
    }

    public class ProblemAccountDataAccess : IProblemAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public ProblemAccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime)
        {
            var criteria = new ProblemAccountsGetCriteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            return repository.Fetch<ProblemAccountsEntity>(criteria);
        }

        public IEnumerable<ProblemAccountsEntity> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime)
        {
            var criteria = new ProblemAccountsCN88GetCriteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            return repository.Fetch<ProblemAccountsEntity>(criteria);
        }

        public InsertCategoryResultEntity InsertProblemAccount(string problemAccountJson, bool isRescan)
        {
            var criteria = new ProblemAccountInsertCriteria
            {
                ProblemAccountJson = problemAccountJson,
                IsRescan = isRescan
            };

            var custIds = repository.Fetch<long>(criteria);

            return new InsertCategoryResultEntity
            {
                ErrorMessage = criteria.ErrorMessage,
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                CustIds = custIds ?? new List<long>()
            };
        }
    }
}