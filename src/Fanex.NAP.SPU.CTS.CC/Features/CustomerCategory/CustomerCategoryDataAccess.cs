namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerCategoryDataAccess
    {
        CustomerCategoryQueueModel GetCustomerCategoryQueue();

        IEnumerable<LatestClassificationEntity> AddCustomerCategory(CustomerCategoryQueueModel model);
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
                CategoryID = x.CategoryID,
                SubscriberID = x.SubscriberID,
                CreatedBy = x.CreatedBy,
                Remark = x.Remark
            });

            var criteria = new AddCustomerCategoryCriteria
            {
                FromAction = (int)FromAction.Service,
                UplineRoleID = model.QueueInfo.UplineRoleID,
                AffectDownline = true,
                QueueID = model.QueueInfo.QueueID,
                CustJson = JsonConvert.SerializeObject(custList)
            };

            return repository.Fetch<LatestClassificationEntity>(criteria);
        }
    }
}
