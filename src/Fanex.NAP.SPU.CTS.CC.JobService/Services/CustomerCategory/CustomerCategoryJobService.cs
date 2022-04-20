namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICustomerCategoryJobService
    {
        Task<bool> Execute();
    }

    public class CustomerCategoryJobService : ICustomerCategoryJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly ICustomerCategoryService customerCategoryService;

        public CustomerCategoryJobService(ISentryClient sentryClient, ICustomerCategoryService customerCategoryService)
        {
            this.sentryClient = sentryClient;
            this.customerCategoryService = customerCategoryService;
            this.serviceName = "CustomerCategoryQueue";
        }

        public Task<bool> Execute()
        {
            try
            {
                var custCategoryQueues = customerCategoryService.GetCustomerCategoryQueue();

                if (custCategoryQueues != null && custCategoryQueues.CustomerCategoryQueue.Any())
                {
                    customerCategoryService.AddCustomerCategory(custCategoryQueues);
                }                
            }
            catch (Exception ex)
            {
                //Utilities.LogSentryError(serviceName, ex.Message, logData, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
