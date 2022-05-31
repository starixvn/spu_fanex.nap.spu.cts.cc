namespace Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface IAffectedCustomerClassificationDataAccess
    {
        IEnumerable<LatestClassificationEntity> InsertVVIPCategory(VVIPCustomerInsertEntity custInfo);

    }

    public class AffectedCustomerClassificationDataAccess : IAffectedCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public AffectedCustomerClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        //public CustomerCategoryQueueModel GetCustomerCategoryQueue()
        //{
        //    //var criteria = new GetCustomerCategoryQueueCriteria();

        //    //using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        //    //{
        //    //    var result = db.QueryMultiple(criteria);

        //    //    var customerCategoryQueue = result.Read<CustomerCategoryEntity>();
        //    //    var queueInfo = result.Read<QueueInfoEntity>();

        //    //    if (customerCategoryQueue.Any() && queueInfo.Any())
        //    //    {
        //    //        return new CustomerCategoryQueueModel
        //    //        {
        //    //            CustomerCategoryQueue = customerCategoryQueue,
        //    //            QueueInfo = queueInfo?.FirstOrDefault()
        //    //        };
        //    //    }                
        //    //}

        //    return null;
        //}

        //public IEnumerable<LatestClassificationEntity> AddCustomerCategory(CustomerCategoryQueueModel model)
        //{
        //    var custList = model.CustomerCategoryQueue.Select(x => new VVIPInsertEntity
        //    {
        //        CustID = x.CustID,
        //        CTSCustID = x.CTSCustID,
        //        RoleID = x.RoleID,
        //        SubscriberID = x.SubscriberID,
        //        CreatedBy = x.CreatedBy,
        //        Remark = x.Remark
        //    });            

        //    var criteria = new VVIPInsertCriteria
        //    {
        //        FromAction = (int)FromAction.Service,
        //        UplineRoleID = model.QueueInfo.UplineRoleID,
        //        AffectDownline = true,
        //        QueueID = model.QueueInfo.QueueID,
        //        CustJson = JsonConvert.SerializeObject(custList)
        //    };

        //    return repository.Fetch<LatestClassificationEntity>(criteria);
        //}

        public IEnumerable<LatestClassificationEntity> InsertVVIPCategory(VVIPCustomerInsertEntity custInfo)
        {
            var criteria = new VVIPInsertCriteria
            {
                FromAction = (int)custInfo.FromAction,
                QueueID = custInfo.QueueID,
                IsAffectDownline = custInfo.IsAffectDownline,
                UplineRoleID = custInfo.UplineRoleID,
                CustJson = JsonConvert.SerializeObject(custInfo)
            };

            return repository.Fetch<LatestClassificationEntity>(criteria) ?? Enumerable.Empty<LatestClassificationEntity>();
        }


        //public IEnumerable<LatestClassificationEntity> RemoveCustomerCategory(CustomerCategoryQueueModel model)
        //{
        //    var custList = model.CustomerCategoryQueue.Select(x => new RemoveCustomerCategoryInfoEntity
        //    {
        //        CustID = x.CustID,
        //        CTSCustID = x.CTSCustID,
        //        CreatedBy = x.CreatedBy                
        //    });

        //    var criteria = new RemoveCustomerCategoryCriteria
        //    {
        //        FromAction = (int)VVIP_FromAction.ServiceDownline,
        //        UplineRoleID = model.QueueInfo.UplineRoleID,
        //        QueueID = model.QueueInfo.QueueID,
        //        AffectDownline = true,
        //        CustJson = JsonConvert.SerializeObject(custList)
        //    };

        //    return repository.Fetch<LatestClassificationEntity>(criteria);
        //}
    }
}
