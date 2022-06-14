namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
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

        IEnumerable<LatestClassificationEntity> InsertVVIPCategory(VVIPCustomerInsertEntity custInfo);

        IEnumerable<VVIPCustomerRemoveResultEntity> RemoveVVIPCategory(VVIPCustomerInsertEntity model);

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

        public IEnumerable<LatestClassificationEntity> InsertVVIPCategory(VVIPCustomerInsertEntity custInfo)
        {
            var criteria = new VVIPCategoryInsertCriteria
            {
                FromAction = (int)custInfo.FromAction,
                QueueID = custInfo.QueueID,
                IsAffectDownline = custInfo.IsAffectDownline,
                UplineRoleID = custInfo.UplineRoleID,
                CustJson = JsonConvert.SerializeObject(custInfo.CustInfo)
            };

            return repository.Fetch<LatestClassificationEntity>(criteria) ?? Enumerable.Empty<LatestClassificationEntity>();
        }

        public IEnumerable<VVIPCustomerRemoveResultEntity> RemoveVVIPCategory(VVIPCustomerInsertEntity model)
        {
            var custInfo = model.CustInfo.Select(x => new
            {
                CustID = x.CustID,
                CTSCustID = x.CTSCustID,
                CreatedBy = x.CreatedBy
            });

            var criteria = new VVIPCategoryRemoveCriteria
            {
                FromAction = (int)VVIP_FromAction.Service,
                UplineRoleID = model.UplineRoleID,
                QueueID = model.QueueID,
                AffectDownline = true,
                CustJson = JsonConvert.SerializeObject(custInfo)
            };

            return repository.Fetch<VVIPCustomerRemoveResultEntity>(criteria);
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