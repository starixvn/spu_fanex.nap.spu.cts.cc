namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerEvidenceInsertDataAccess
    {
        IEnumerable<CustomerEvidenceFromFileEntity> GetCustomerEvidenceFromFile();

        ResultRemoveCustomerEvidence RemoveCustomerEvidenceByCTSCustIds(string jsonData, bool isRollBack, bool isClearLog);

        DBResult InsertCustomerEvidenceMuitiple(string jsonData, bool isOverwrite);

        DBResult SyncCustomerEvidenceToCategory(string jsonData, bool isRescan);

        DBResult UpdateProcessedCustEvidence(string idList, bool isProcessed);

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

        public ResultRemoveCustomerEvidence RemoveCustomerEvidenceByCTSCustIds(string jsonData, bool isRollBack, bool isClearLog)
        {
            var criteria = new CustEvidencesByCTSCustIdsRemoveCriteria
            {
                NewAccountEvidences = jsonData,
                SessionId = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
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

        public DBResult InsertCustomerEvidenceMuitiple(string jsonData, bool isOverwrite)
        {
            var criteria = new CustEvidencesInsertCriteria
            {
                NewAccountEvidences = jsonData,
                UserId = ImportEvidence_StarixITId,
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

        public DBResult UpdateProcessedCustEvidence(string idList, bool isProcessed)
        {
            var criteria = new ProcessedCustEvidenceUpdateCriteria
            {
                IdList = idList,
                IsProcessed = isProcessed,
                UserId = ImportEvidence_StarixITId
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
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