namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
    using Microsoft.AspNetCore.Mvc;
    
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/classificationBySportJobs")]
    public class ClassificationBySportJobController : ControllerBase
    {
        private readonly INormalAccountClassificationBySportJobService _normalAccountClassificationBySportJobService;
        private readonly INormalAccountInsertBySportJobService _normalAccountInsertBySportJobService;
        private readonly IRealtimeClassificationBySportJobService _realtimeClassificationBySportJobService;
        private readonly IProbationClassificationBySportJobService _probationClassificationBySportJobService;
        private readonly IDailyNormalClassificationBySportJobService _dailyNormalClassificationBySportJobService;
        private readonly IDailyProblemClassificationBySportJobService _dailyProblemClassificationBySportJobService;
        private readonly IMatchMonitorClassificationBySportJobService mmClassificationBySportJobService;

        public ClassificationBySportJobController(
            INormalAccountClassificationBySportJobService normalAccountClassificationBySportJobService,
            INormalAccountInsertBySportJobService normalAccountInsertBySportJobService,
            IRealtimeClassificationBySportJobService realtimeClassificationBySportJobService,
            IProbationClassificationBySportJobService probationClassificationBySportJobService,
            IDailyNormalClassificationBySportJobService dailyNormalClassificationBySportJobService,
            IMatchMonitorClassificationBySportJobService mmClassificationBySportJobService)
        {
            this.normalAccountClassificationBySportJobService = normalAccountClassificationBySportJobService;
            this.normalAccountInsertBySportJobService = normalAccountInsertBySportJobService;
            this.realtimeClassificationBySportJobService = realtimeClassificationBySportJobService;
            this.probationClassificationBySportJobService = probationClassificationBySportJobService;
            this.dailyNormalClassificationBySportJobService = dailyNormalClassificationBySportJobService;
            this.mmClassificationBySportJobService = mmClassificationBySportJobService;
        }

        [HttpPost("NormalAccountClassificationBySport")]
        public async Task<bool> NormalAccountClassificationBySport([FromForm] int batchInternalSize)
        {
            return await _normalAccountClassificationBySportJobService.Execute(batchInternalSize);
        }

        [HttpPost("NormalAccountInsertBySport")]
        public async Task<bool> NormalAccountInsertBySport([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await _normalAccountInsertBySportJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("ScanRealtimeClassificationBySport")]
        public async Task<bool> ScanRealtimeClassificationBySport([FromForm] int batchInternalSize)
        {
            return await _realtimeClassificationBySportJobService.Execute(batchInternalSize);
        }

        [HttpPost("ScanProbationClassificationBySport")]
        public async Task<bool> ScanProbationClassificationBySport()
        {
            return await _probationClassificationBySportJobService.Execute();
        }

        [HttpPost("ScanningDailyNormalClassificationBySport")]
        public async Task<bool> ScanningDailyNormalClassificationBySport()
        {
            return await _dailyNormalClassificationBySportJobService.Execute();
        }

        [HttpPost("ScanDailyProblemClassiciationBySport")]
        public async Task<bool> ScanProblemClassificationBySport([FromForm] int batchSize)
        {
            return await _dailyProblemClassificationBySportJobService.Excute(batchSize);
        }
        [HttpPost("ClassifyMatchMonitorBySport")]
        public async Task<bool> ClassifyMatchMonitorBySport()
        {
            return await mmClassificationBySportJobService.ClassifyMatchMonitorBySport();
        }
    }
}
