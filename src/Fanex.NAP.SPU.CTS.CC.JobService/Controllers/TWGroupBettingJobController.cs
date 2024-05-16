namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
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

        [HttpPost("ScanTWGBTicket")]
        public async Task<bool> ScanTWGBTicket([FromForm] int batchSize)
        {
            return await this.twGroupBettingJobService.ScanTWGBTicket(batchSize);
        }

        [HttpPost("UpdateTotalTWGBTicket")]
        public async Task<bool> UpdateTotalTWGBTicket()
        {
            return await this.twGroupBettingJobService.UpdateTotalTWGBTicket();
        }
    }
}