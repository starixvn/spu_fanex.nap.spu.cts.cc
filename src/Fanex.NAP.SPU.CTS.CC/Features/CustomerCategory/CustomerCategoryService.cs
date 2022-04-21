namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory
{
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICustomerCategoryService
    {
        CustomerCategoryQueueModel GetCustomerCategoryQueue();

        IEnumerable<LatestClassificationEntity> AddCustomerCategory(CustomerCategoryQueueModel model);
    }

    public class CustomerCategoryService : ICustomerCategoryService
    {
        private readonly ICustomerCategoryDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public CustomerCategoryService(ICustomerCategoryDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public CustomerCategoryQueueModel GetCustomerCategoryQueue()
        {
            return dataAccess.GetCustomerCategoryQueue();
        }

        public IEnumerable<LatestClassificationEntity> AddCustomerCategory(CustomerCategoryQueueModel model)
        {
            return dataAccess.AddCustomerCategory(model);
        }
        
    }
}
