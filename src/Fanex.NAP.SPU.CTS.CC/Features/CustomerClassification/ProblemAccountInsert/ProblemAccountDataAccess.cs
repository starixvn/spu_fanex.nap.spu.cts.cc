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
        IEnumerable<ExistedProblemAccountEntity> GetExistedProblemAccounts(string custIds);

        InsertPAResultEntity InsertProblemAccount(string problemAccountJson, string byType, bool isFromQueue);

        IEnumerable<CustInfoEntity> InsertRobotAccount(string robotAccountJson);

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

        public IEnumerable<ExistedProblemAccountEntity> GetExistedProblemAccounts(string custIds)
        {
            var criteria = new ProblemAccountGetCriteria()
            {
                CustIds = custIds,
            };

            return repository.Fetch<ExistedProblemAccountEntity>(criteria) ?? Enumerable.Empty<ExistedProblemAccountEntity>();
        }

        public InsertPAResultEntity InsertProblemAccount(string problemAccountJson, string byType, bool isFromQueue)
        {
            var criteria = new ProblemAccountInsertCriteria
            {
                ProblemAccountJson = problemAccountJson,
                ByType = byType,
                IsFromQueue = isFromQueue
            };

            var result = repository.Fetch<CustInfoEntity>(criteria);

            return new InsertPAResultEntity
            {
                ErrorMessage = criteria.ErrorMessage,
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                CustomerInfo = result ?? Enumerable.Empty<CustInfoEntity>()
            };
        }

        public IEnumerable<CustInfoEntity> InsertRobotAccount(string robotAccountJson)
        {
            var criteria = new RobotAccountInsertCriteria
            {
                CustInfo = robotAccountJson
            };

            return repository.Fetch<CustInfoEntity>(criteria) ?? Enumerable.Empty<CustInfoEntity>();
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
            var criteria = new BetLimitByEvidenceUpdateCriteria
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
            var criteria = new BetLimitByEvidenceRemoveCriteria
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