namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/twgroupbetting")]
    public class TWGroupBettingJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly ITWGroupBettingJobService twGroupBettingJobService;


        public TWGroupBettingJobController(ISentryClient sentryClient,
            ITWGroupBettingJobService twGroupBettingJobService)
        {
            this.sentryClient = sentryClient;
            this.twGroupBettingJobService = twGroupBettingJobService;
        }

        [HttpPost("scan-tw-group-betting")]
        public async Task<bool> ScanTWGroupBetting()
        {
            return await this.twGroupBettingJobService.DetectGroupBettingTicket();
        }

        [HttpPost("count-cust-total-ticket")]
        public async Task<bool> CountCustomerTotalTicket()
        {
            return await this.twGroupBettingJobService.CountTotalTicket();
        }
    }
}
