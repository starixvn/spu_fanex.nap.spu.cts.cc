namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections.Generic;

    public interface ICustomerEvidenceInsertDataAccess
    {
        IEnumerable<CustomerEvidenceFromFileEntity> GetCustomerEvidenceFromFile();

        ResultRemoveCustomerEvidence RemoveCustomerEvidenceByCTSCustIds(string jsonData, long sessionId, bool isRollBack, bool isClearLog);

        DBResult InsertCustomerEvidenceMuitiple(string jsonData, int userId, bool isOverwrite);

        DBResult SyncCustomerEvidenceToCategory(string jsonData, bool isRescan);

        DBResult UpdateProcessedCustEvidence(string idList, bool isProcessed, int userId);

        IEnumerable<int> GetAssociatedAccountByCTSCustIds(string ctsCustIds);

        DBResult UpdateBetLimitOfFlaggedCustomer(string custIds);
    }

    public class CustomerEvidenceInsertDataAccess : ICustomerEvidenceInsertDataAccess
    {
        private readonly IDynamicRepository repository;

        public CustomerEvidenceInsertDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustomerEvidenceFromFileEntity> GetCustomerEvidenceFromFile()
        {
            var criteria = new CustomerEvidenceFromFileGetCriteria();

            return repository.Fetch<CustomerEvidenceFromFileEntity>(criteria);
        }

        public ResultRemoveCustomerEvidence RemoveCustomerEvidenceByCTSCustIds(string jsonData, long sessionId, bool isRollBack, bool isClearLog)
        {
            var criteria = new CustEvidencesByCTSCustIdsRemoveCriteria
            {
                NewAccountEvidences = jsonData,
                SessionId = sessionId,
                IsRollBack = isRollBack,
                IsClearLog = isClearLog
            };

            var result = new ResultRemoveCustomerEvidence
            {
                CustEviRemovedList = repository.Fetch<CustomerEvidenceRemovedEntity>(criteria),
                DBResult = new DBResult
                {
                    ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                    ErrorMsg = criteria.ErrorMessage
                }
            };

            return result;
        }

        public DBResult InsertCustomerEvidenceMuitiple(string jsonData, int userId, bool isOverwrite)
        {
            var criteria = new CustEvidencesInsertCriteria
            {
                NewAccountEvidences = jsonData,
                UserId = userId,
                IsOverride = isOverwrite
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult SyncCustomerEvidenceToCategory(string jsonData, bool isRescan)
        {
            var criteria = new CustEvidenceToCTSCategorySyncCriteria
            {
                CustEvidences = jsonData,
                IsRescan = isRescan
            };

            if (!string.IsNullOrEmpty(jsonData))
            {
                repository.Execute(criteria);
            }

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult UpdateProcessedCustEvidence(string idList, bool isProcessed, int userId)
        {
            var criteria = new ProcessedCustEvidenceUpdateCriteria
            {
                IdList = idList,
                IsProcessed = isProcessed,
                UserId = userId
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public IEnumerable<int> GetAssociatedAccountByCTSCustIds(string ctsCustIds)
        {
            var criteria = new AssociatedAccountByCTSCustIdsGetCriteria
            {
                CTSCustIdList = ctsCustIds
            };

            return repository.Fetch<int>(criteria);
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
    }
}