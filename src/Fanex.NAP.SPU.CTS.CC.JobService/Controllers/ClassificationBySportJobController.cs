namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
    using Microsoft.AspNetCore.Mvc;
    
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/classificationBySportJobs")]
    public class ClassificationBySportJobController : ControllerBase
    {
        private readonly INormalAccountClassificationBySportJobService normalAccountClassificationBySportJobService;
        private readonly INormalAccountInsertBySportJobService normalAccountInsertBySportJobService;
        private readonly IRealtimeClassificationBySportJobService realtimeClassificationBySportJobService;
        private readonly IProbationClassificationBySportJobService probationClassificationBySportJobService;
        private readonly IDailyNormalClassificationBySportJobService dailyNormalClassificationBySportJobService;

        public ClassificationBySportJobController(
            INormalAccountClassificationBySportJobService normalAccountClassificationBySportJobService,
            INormalAccountInsertBySportJobService normalAccountInsertBySportJobService,
            IRealtimeClassificationBySportJobService realtimeClassificationBySportJobService,
            IProbationClassificationBySportJobService probationClassificationBySportJobService,
            IDailyNormalClassificationBySportJobService dailyNormalClassificationBySportJobService)
        {
            this.normalAccountClassificationBySportJobService = normalAccountClassificationBySportJobService;
            this.normalAccountInsertBySportJobService = normalAccountInsertBySportJobService;
            this.realtimeClassificationBySportJobService = realtimeClassificationBySportJobService;
            this.probationClassificationBySportJobService = probationClassificationBySportJobService;
            this.dailyNormalClassificationBySportJobService = dailyNormalClassificationBySportJobService;
        }

        [HttpPost("NormalAccountClassificationBySport")]
        public async Task<bool> NormalAccountClassificationBySport([FromForm] int batchInternalSize)
        {
            return await this.normalAccountClassificationBySportJobService.Execute(batchInternalSize);
        }

        [HttpPost("NormalAccountInsertBySport")]
        public async Task<bool> NormalAccountInsertBySport([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountInsertBySportJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("ScanRealtimeClassificationBySport")]
        public async Task<bool> ScanRealtimeClassificationBySport([FromForm] int batchInternalSize)
        {
            return await this.realtimeClassificationBySportJobService.Execute(batchInternalSize);
        }

        [HttpPost("ScanProbationClassificationBySport")]
        public async Task<bool> ScanProbationClassificationBySport()
        {
            return await this.probationClassificationBySportJobService.Execute();
        }

        [HttpPost("ScanningDailyNormalClassificationBySport")]
        public async Task<bool> ScanningDailyNormalClassificationBySport()
        {
            return await this.dailyNormalClassificationBySportJobService.Execute();
        }

    }
}
