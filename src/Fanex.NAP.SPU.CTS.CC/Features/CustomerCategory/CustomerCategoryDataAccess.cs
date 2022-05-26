namespace Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;    
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerCategoryDataAccess
    {
        CustomerCategoryQueueModel GetCustomerCategoryQueue();

        IEnumerable<LatestClassificationEntity> AddCustomerCategory(CustomerCategoryQueueModel model);

        IEnumerable<LatestClassificationEntity> AddCustomerCategory(IEnumerable<CustInfoVVipModel> model);

        IEnumerable<LatestClassificationEntity> RemoveCustomerCategory(CustomerCategoryQueueModel model);
    }

    public class CustomerCategoryDataAccess : ICustomerCategoryDataAccess
    {
        private readonly IDynamicRepository repository;

        public CustomerCategoryDataAccess()
        {
            repository = new DynamicRepository();
        }

        public CustomerCategoryQueueModel GetCustomerCategoryQueue()
        {
            var criteria = new GetCustomerCategoryQueueCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.QueryMultiple(criteria);

                var customerCategoryQueue = result.Read<CustomerCategoryEntity>();
                var queueInfo = result.Read<QueueInfoEntity>();

                if (customerCategoryQueue.Any() && queueInfo.Any())
                {
                    return new CustomerCategoryQueueModel
                    {
                        CustomerCategoryQueue = customerCategoryQueue,
                        QueueInfo = queueInfo?.FirstOrDefault()
                    };
                }                
            }

            return new CustomerCategoryQueueModel();
        }

        public IEnumerable<LatestClassificationEntity> AddCustomerCategory(CustomerCategoryQueueModel model)
        {
            var custList = model.CustomerCategoryQueue.Select(x => new AddCustomerCategoryInfoEntity
            {
                CustID = x.CustID,
                CTSCustID = x.CTSCustID,
                RoleID = x.RoleID,
                SubscriberID = x.SubscriberID,
                CreatedBy = x.CreatedBy,
                Remark = x.Remark
            });            

            var criteria = new AddCustomerCategoryCriteria
            {
                FromAction = (int)VVIP_FromAction.Service,
                UplineRoleID = model.QueueInfo.UplineRoleID,
                AffectDownline = true,
                QueueID = model.QueueInfo.QueueID,
                CustJson = JsonConvert.SerializeObject(custList)
            };

            return repository.Fetch<LatestClassificationEntity>(criteria);
        }

        public IEnumerable<LatestClassificationEntity> AddCustomerCategory(IEnumerable<CustInfoVVipModel> model)
        {
            var criteria = new AddCustomerCategoryCriteria
            {
                FromAction = (int)VVIP_FromAction.ServiceInsertNew,                                
                CustJson = JsonConvert.SerializeObject(model)
            };

            return repository.Fetch<LatestClassificationEntity>(criteria);
        }

        public IEnumerable<LatestClassificationEntity> RemoveCustomerCategory(CustomerCategoryQueueModel model)
        {
            var custList = model.CustomerCategoryQueue.Select(x => new RemoveCustomerCategoryInfoEntity
            {
                CustID = x.CustID,
                CTSCustID = x.CTSCustID,
                CreatedBy = x.CreatedBy                
            });

            var criteria = new RemoveCustomerCategoryCriteria
            {
                FromAction = (int)VVIP_FromAction.Service,
                UplineRoleID = model.QueueInfo.UplineRoleID,
                QueueID = model.QueueInfo.QueueID,
                AffectDownline = true,
                CustJson = JsonConvert.SerializeObject(custList)
            };

            return repository.Fetch<LatestClassificationEntity>(criteria);
        }
    }
}
