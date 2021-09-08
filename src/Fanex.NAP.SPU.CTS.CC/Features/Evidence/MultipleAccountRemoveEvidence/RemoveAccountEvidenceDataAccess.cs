namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections;
    using System.Collections.Generic;

    public interface IRemoveAccountEvidenceDataAccess
    {
        IEnumerable<CustomerEvidenceQueueEntity> GetCustomerEvidenceQueue();

        DBResult ModifyCustomerEvidence(string customerEvidence, int actionId);

        DBResult UpdateCustomerEvidence(string processedIdList);
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

        public DBResult ModifyCustomerEvidence(string customerEvidence, int actionId)
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

        public DBResult UpdateCustomerEvidence(string processedIdList)
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
    }
}