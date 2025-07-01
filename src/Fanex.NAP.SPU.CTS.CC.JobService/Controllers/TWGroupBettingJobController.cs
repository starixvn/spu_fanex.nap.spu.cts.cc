namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/twgroupbetting")]
    public class TWGroupBettingJobController : ControllerBase
    {
        private readonly ITWGroupBettingJobService twGroupBettingJobService;

        public TWGroupBettingJobController(
            ITWGroupBettingJobService twGroupBettingJobService)
        {
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