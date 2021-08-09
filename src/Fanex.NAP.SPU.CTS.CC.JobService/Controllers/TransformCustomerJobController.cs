namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
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
        public async Task<bool> InsertNewCustomer()
        {
            sentryClient.CaptureMessage($"InsertNewCustomer is running at {DateTime.Now}");
            return await ctsCustomerJobService.InsertNewCustomer();
        }

        [HttpPost("UpdateCTSCustomerInfo")]
        public async Task<bool> UpdateCTSCustomerInfo([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"UpdateCTSCustomerInfo is running at {DateTime.Now}");
            return await ctsCustomerJobService.UpdateCTSCustomerInfo(batchSize);
        }
        
        [HttpPost("UpdateUsername2")]
        public async Task<bool> UpdateUsername2([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"UpdateUsername2 is running at {DateTime.Now}");
            return await ctsCustomerJobService.UpdateUsername2(batchSize);
        }
    }
}