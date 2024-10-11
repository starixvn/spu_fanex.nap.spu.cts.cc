namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICustomerClassificationAgencyService
    {
        Task<bool> InsertProblem();

        Task<bool> RemoveProblem();

        Task<bool> InsertVVIP();

        Task<bool> RemoveVVIP();
       
    }

    public class CustomerClassificationAgencyService : ICustomerClassificationAgencyService
    {
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "CustomerClassificationAgencyService";

        public CustomerClassificationAgencyService()
        {
            this.sentryClient = sentryClient;
        }

        public async Task<bool> InsertProblem()
        {
            return true;
        }

        public async Task<bool> RemoveProblem()
        {
            return true;
        }

        public async Task<bool> InsertVVIP()
        {
            return true;
        }

        public async Task<bool> RemoveVVIP()
        {
            return true;
        }

        private static async Task<bool> CallAPI(IEnumerable<long> custIds, string url)
        {
            if (custIds != null && custIds.Any())
            {
                var para = new
                {
                    CustIds = custIds,                   
                };

                return await ApiHelper.Post<bool>(url, para, 300);
            }

            return true;
        }
    }
}
