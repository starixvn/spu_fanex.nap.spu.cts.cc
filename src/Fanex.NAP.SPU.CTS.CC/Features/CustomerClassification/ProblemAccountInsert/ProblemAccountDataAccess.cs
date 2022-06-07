namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountDataAccess
    {
        IEnumerable<ExistedProblemAccountEntity> GetExistedProblemAccounts(string custIds);

        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(string custIds, FunctionId funcId);

        IEnumerable<CustInfoEntity> InsertProblemAccount(string problemAccountJson, string inputType, bool isFromDownlineQueue);

        RemovePAResultEntity RemoveProblemAccount(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue);

        IEnumerable<CustInfoEntity> InsertRobotAccount(string robotAccountJson);

        void RemoveOldProblemAccount(string custIds);

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

        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(string custIds, FunctionId funcId)
        {
            var criteria = new ProblemAccountLosingClassificationGetCriteria()
            {
                ListCustIds = custIds,
                FunctionId = (int)funcId
            };

            return repository.Fetch<ProblemAccountLosingClassificationEntity>(criteria);
        }


        public IEnumerable<CustInfoEntity> InsertProblemAccount(string problemAccountJson, string inputType, bool isFromDownlineQueue)
        {
            var criteria = new ProblemAccountInsertCriteria
            {
                ProblemAccountJson = problemAccountJson,
                InputType = inputType,
                IsFromQueue = isFromDownlineQueue
            };

            return repository.Fetch<CustInfoEntity>(criteria);
        }

        public RemovePAResultEntity RemoveProblemAccount(IEnumerable<RemoveProblemAccountEntity> problemAccounts, bool isFromQueue)
        {
            var criteria = new ProblemAccountRemoveCriteria
            {
                ProblemAccountJson = JsonConvert.SerializeObject(problemAccounts),
                IsFromQueue = isFromQueue
            };

            var custInfos = repository.Fetch<CustRemovePAEntity>(criteria);

            return new RemovePAResultEntity
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMessage = criteria.ErrorMessage,
                CustomerInfo = custInfos ?? Enumerable.Empty<CustRemovePAEntity>()
            };
        }

        public void RemoveOldProblemAccount(string custIds)
        {
            var criteria = new OldProblemAccountRemoveCriteria
            {
                CustId = custIds
            };

            repository.Execute(criteria);
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