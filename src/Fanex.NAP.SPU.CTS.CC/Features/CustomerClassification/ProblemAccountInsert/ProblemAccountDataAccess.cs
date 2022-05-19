namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProblemAccountDataAccess
    {
        IEnumerable<ProblemAccountEntity> GetExistedProblemAccounts(string custIds);

        InsertPAResultModel InsertProblemAccountByCategory(string problemAccountJson);

        int CheckNewPAAssociationCustomerForRollback(string custInfoJson);

        DBResult UpdateBetLimitOfFlaggedCustomer(string custIds);

        DBResult UpdateBetLimitOfUnflaggedCustomer(string custIds);
    }

    public class ProblemAccountDataAccess : IProblemAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public ProblemAccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<ProblemAccountEntity> GetExistedProblemAccounts(string custIds)
        {
            var criteria = new ProblemAccountGetCriteria()
            {
                CustIds = custIds,
            };

            return repository.Fetch<ProblemAccountEntity>(criteria) ?? Enumerable.Empty<ProblemAccountEntity>();
        }

        public InsertPAResultModel InsertProblemAccountByCategory(string problemAccountJson)
        {
            var criteria = new ProblemAccountByCategoryInsertCriteria
            {
                CustInfoJson = problemAccountJson
            };

            var result = repository.Fetch<AssCustomerForCheckingRollbackEntity>(criteria);

            return new InsertPAResultModel
            {
                ErrorMessage = criteria.ErrorMessage,
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                customerInfo = result ?? Enumerable.Empty<AssCustomerForCheckingRollbackEntity>()
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

        public DBResult UpdateBetLimitOfFlaggedCustomer(string custIds)
        {
            var criteria = new BetLimitByEvidenceAddingUpdateCriteria
            {
                CustIDList = custIds
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult UpdateBetLimitOfUnflaggedCustomer(string custIds)
        {
            var criteria = new BetLimitByEvidenceRemovingUpdateCriteria
            {
                CustIDList = custIds
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }
    }
}