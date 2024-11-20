namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownlineCustomerClassification;
    using System;
    using System.Linq;

    public interface IAffectedDownlineCustomerClassificationDataAccess
    {
        CustClassificationFromQueueModel GetCustomerCategoryFromQueue();

        void CustomerClassificationFromQueueComplete(UInt64 queueID, Int64 maxCustId);
    }

    public class AffectedDownlineCustomerClassificationDataAccess : IAffectedDownlineCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public AffectedDownlineCustomerClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public CustClassificationFromQueueModel GetCustomerCategoryFromQueue()
        {
            var criteria = new CustClassificationQueueGetCriteria();

            var (memberQueue, agencyQueue) = repository.FetchMultiple<CustClassificationFromQueueEntity, CustClassificationFromQueueEntity>(criteria);

            return new CustClassificationFromQueueModel
            {
                MemberQueue = memberQueue ?? Enumerable.Empty<CustClassificationFromQueueEntity>(),
                AgencyQueue = agencyQueue ?? Enumerable.Empty<CustClassificationFromQueueEntity>(),
                ActionType = criteria.ActionType,
                QueueID = criteria.QueueID,
                UplineRoleID = criteria.UplineRoleID,
                LastDownlineCTSCustID = criteria.LastDownlineCTSCustID
            };
        }

        public void CustomerClassificationFromQueueComplete(UInt64 queueID, Int64 maxCustId)
        {
            var criteria = new CustClassificationQueueCompleteCriteria
            {
                QueueID = queueID,
                MaxCustID = maxCustId
            };

            repository.Execute(criteria);
        }
    }
}