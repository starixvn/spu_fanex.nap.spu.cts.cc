namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAffectedDownlineCustomerClassificationDataAccess
    {
        CustClassificationFromQueueResultEntity GetCustomerCategoryFromQueue();

        void CustomerClassificationFromQueueComplete(UInt64 queueID, Int64 maxCustId);
    }

    public class AffectedDownlineCustomerClassificationDataAccess : IAffectedDownlineCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public AffectedDownlineCustomerClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public CustClassificationFromQueueResultEntity GetCustomerCategoryFromQueue()
        {
            var queueResult = new CustClassificationFromQueueResultEntity();

            var criteria = new CustClassificationQueueGetCriteria();

            queueResult.CustomerClassificationQueue = repository.Fetch<CustClassificationFromQueueEntity>(criteria) ?? Enumerable.Empty<CustClassificationFromQueueEntity>();
            queueResult.ActionType = criteria.ActionType;
            queueResult.QueueID = criteria.QueueID;
            queueResult.UplineRoleID = criteria.UplineRoleID;
            queueResult.LastDownlineCTSCustID = criteria.LastDownlineCTSCustID;

            return queueResult;
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