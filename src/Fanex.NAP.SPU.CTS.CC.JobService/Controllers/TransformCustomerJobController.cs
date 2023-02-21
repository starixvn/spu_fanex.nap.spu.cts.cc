namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/TransformCustomer")]
    public class TransformCustomerJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly ICTSCustomerJobService ctsCustomerJobService;

        public TransformCustomerJobController(
            ISentryClient sentryClient,
            ICTSCustomerJobService ctsCustomerJobService)
        {
            this.sentryClient = sentryClient;
            this.ctsCustomerJobService = ctsCustomerJobService;
        }

        [HttpPost("InsertNewCustomer")]
        public async Task<bool> InsertNewCustomer([FromForm] int batchSize)
        {
            return await ctsCustomerJobService.InsertNewCustomer(batchSize);
        }

        [HttpPost("UpdateCTSCustomerInfo")]
        public async Task<bool> UpdateCTSCustomerInfo([FromForm] int batchSize)
        {
            return await ctsCustomerJobService.UpdateCTSCustomerInfo(batchSize);
        }

        [HttpPost("UpdateUsername2")]
        public async Task<bool> UpdateUsername2([FromForm] int batchSize)
        {
            return await ctsCustomerJobService.UpdateUsername2(batchSize);
        }

        [HttpPost("MonitorMissingCustomer")]
        public async Task<bool> MonitorMissingCustomer([FromForm] int batchSize)
        {
            return await ctsCustomerJobService.MonitorMissingCustomer(batchSize);
        }

        [HttpPost("InsertReactivatedCustomer")]
        public async Task<bool> InsertReactivatedCustomer()
        {
            return await ctsCustomerJobService.InsertReactivatedCustomers();
        }

        [HttpPost("InactivateNewCustomers")]
        public async Task<bool> InactivateNewCustomers()
        {
            return await ctsCustomerJobService.InactivateNewCustomers();
        }
    }
}