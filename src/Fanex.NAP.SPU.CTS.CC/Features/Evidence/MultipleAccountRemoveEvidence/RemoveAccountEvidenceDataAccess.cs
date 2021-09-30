namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections;
    using System.Collections.Generic;

    public interface IRemoveAccountEvidenceDataAccess
    {
        IEnumerable<CustomerEvidenceQueueEntity> GetCustomerEvidenceQueue();

        DBResult RemoveCustEvidenceFromQueue(string customerEvidence, int actionId);

        DBResult UpdateCustomerEvidenceComplete(string processedIdList);

        DBResult UpdateBetLimitOfUnflaggedCustomer(string custIds);

        IEnumerable<CheckCleanEntity> CheckCleanCustFlaggedEvidence(string ctsCustIds);
    }

    public class RemoveAccountEvidenceDataAccess : IRemoveAccountEvidenceDataAccess
    {
        private readonly IDynamicRepository repository;

        public RemoveAccountEvidenceDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustomerEvidenceQueueEntity> GetCustomerEvidenceQueue()
        {
            var criteria = new CustEvidenceQueueGetCriteria();

            return repository.Fetch<CustomerEvidenceQueueEntity>(criteria);
        }

        public DBResult RemoveCustEvidenceFromQueue(string customerEvidence, int actionId)
        {
            var criteria = new CustomerEvidenceFromQueueModifyCriteria
            {
                CustomerEvidence = customerEvidence,
                ActionId = actionId
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage,
                OutParam = new Hashtable {
                    { "custIds", criteria.CustIdResultList }
                }
            };
        }

        public DBResult UpdateCustomerEvidenceComplete(string processedIdList)
        {
            var criteria = new CustomerEvidenceUpdateCriteria
            {
                ProcessedIdList = processedIdList
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

        public IEnumerable<CheckCleanEntity> CheckCleanCustFlaggedEvidence(string ctsCustIds)
        {
            var criteria = new CleanCustFlaggedEvidenceCheckCriteria
            {
                CTSCustIds = ctsCustIds
            };

            return repository.Fetch<CheckCleanEntity>(criteria);
        }
    }
}