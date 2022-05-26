namespace Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;    
    using System.Collections.Generic;    

    public interface ICustomerCategoryService
    {
        CustomerCategoryQueueModel GetCustomerCategoryQueue();

        IEnumerable<LatestClassificationEntity> AddCustomerCategory(CustomerCategoryQueueModel model);

        IEnumerable<LatestClassificationEntity> AddCustomerCategory(IEnumerable<CustInfoVVipModel> model);

        IEnumerable<LatestClassificationEntity> RemoveCustomerCategory(CustomerCategoryQueueModel model);
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
            Utilities.LogSentryInfo("CustomerCategoryService", "AddCustomerCategory", model, true, sentryClient);

            return dataAccess.AddCustomerCategory(model);
        }

        public IEnumerable<LatestClassificationEntity> AddCustomerCategory(IEnumerable<CustInfoVVipModel> model)
        {
            return dataAccess.AddCustomerCategory(model);
        }

        public IEnumerable<LatestClassificationEntity> RemoveCustomerCategory(CustomerCategoryQueueModel model)
        {
            return dataAccess.RemoveCustomerCategory(model);
        }
    }
}
