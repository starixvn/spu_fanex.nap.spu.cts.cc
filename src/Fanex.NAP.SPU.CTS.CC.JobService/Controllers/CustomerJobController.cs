namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/TransformCustomer")]
    public class CustomerJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly ICTSCustomerJobService ctsCustomerJobService;

        public CustomerJobController(
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

        [HttpPost("UpdateUsername2ForCustMain")]
        public async Task<bool> UpdateUsername2ForCustMain([FromForm] int batchSize)
        {
            return await ctsCustomerJobService.UpdateUsername2ForCustMain(batchSize);
        }

        [HttpPost("UpdateUsername2ForCustSub")]
        public async Task<bool> UpdateUsername2ForCustSub([FromForm] int batchSize)
        {
            return await ctsCustomerJobService.UpdateUsername2ForCustSub(batchSize);
        }

        [HttpPost("InsertReactivatedCustomer")]
        public async Task<bool> InsertReactivatedCustomer()
        {
            return await ctsCustomerJobService.InsertReactivatedCustomers();
        }

        [HttpPost("InactivateNewCustomers")]
        public async Task<bool> InactivateNewCustomers([FromForm] int batchSize)
        {
            return await ctsCustomerJobService.InactivateNewCustomers(batchSize);
        }
    }
}