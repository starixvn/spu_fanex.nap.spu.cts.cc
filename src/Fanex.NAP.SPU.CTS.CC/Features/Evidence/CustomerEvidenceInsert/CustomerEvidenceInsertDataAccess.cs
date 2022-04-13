namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerEvidenceInsertDataAccess
    {
        IEnumerable<CustomerEvidenceFromFileEntity> GetCustomerEvidenceFromFile();

        ResultRemoveCustomerEvidence RemoveCustomerEvidenceByCTSCustIds(string jsonData, bool isRollBack, bool isClearLog);

        DBResult InsertCustomerEvidenceMuitiple(string jsonData, bool isOverwrite, int sourceType);

        SynctEvidenceCategoryResultEntity SyncCustomerEvidenceToCategory(string jsonData, bool isRescan);

        DBResult UpdateProcessedCustEvidence(string idList, bool isProcessed);

        DBResult UpdateBetLimitOfFlaggedCustomer(string custIds);

        IEnumerable<CustEvidenceByReasonIdEntity> GetCustEvidenceByReasonId(string custInfoJson);
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

        public DBResult InsertCustomerEvidenceMuitiple(string jsonData, bool isOverwrite, int sourceType)
        {
            var criteria = new CustEvidencesInsertCriteria
            {
                NewAccountEvidences = jsonData,
                UserId = ImportEvidence_StarixITId,
                IsOverride = isOverwrite,
                SourceType = sourceType
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public SynctEvidenceCategoryResultEntity SyncCustomerEvidenceToCategory(string jsonData, bool isRescan)
        {
            var criteria = new CustEvidenceToCTSCategorySyncCriteria
            {
                CustEvidences = jsonData,
                IsRescan = isRescan
            };

            var custInfos = repository.Fetch<CheckPACustomerAssociationForRollbackEntity>(criteria).ToList();       
           
            return new SynctEvidenceCategoryResultEntity
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage,
                CustInfo = custInfos ?? Enumerable.Empty<CheckPACustomerAssociationForRollbackEntity>()
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

        public IEnumerable<CustEvidenceByReasonIdEntity> GetCustEvidenceByReasonId(string custInfoJson)
        {
            var criteria = new CustEvidenceGetByReasonIdCriteria
            {
                CustInfoJson = custInfoJson
            };

            return repository.Fetch<CustEvidenceByReasonIdEntity>(criteria);
        }

    }
}
