namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProblemAccountDataAccess
    {
        IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime);

        InsertCategoryResultEntity InsertProblemAccount(string problemAccountJson, bool isRescan);

        int CheckNewPAAssociationCustomerForRollback(string custInfoJson);
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

        public InsertCategoryResultEntity InsertProblemAccount(string problemAccountJson, bool isRescan)
        {
            var criteria = new ProblemAccountInsertCriteria
            {
                ProblemAccountJson = problemAccountJson,
                IsRescan = isRescan
            };

            var assCustForRollback = repository.Fetch<CheckPACustomerAssociationForRollbackEntity>(criteria);

            return new InsertCategoryResultEntity
            {
                ErrorMessage = criteria.ErrorMessage,
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                CustIds = assCustForRollback?.Select(x => x.CustID) ?? Enumerable.Empty<long>(),
                NewPAAssociationCustomerForRollback = assCustForRollback
            };
        }

        public int CheckNewPAAssociationCustomerForRollback(string custInfoJson)
        {
            var criteria = new CheckNewPAAssociationCustomerForRollbackCriteria
            {
                CustInfoJson = custInfoJson
            };

            return repository.Execute(criteria);
        }
    }
}