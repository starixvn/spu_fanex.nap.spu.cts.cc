namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/classificationJobs")]
    public class ClassificationJobController : ControllerBase
    {
        private readonly IProblemAccountJobService problemAccountInsertJobService;
        private readonly INormalAccountJobService normalAccountInsertJobService;
        private readonly IDailyClassificationJobService dailyClassificationJobService;
        private readonly IRealtimeClassificationJobService realtimeClassificationJobService;
        private readonly IProbationClassificationJobService probationClassificationJobService;
        private readonly IRobotAccountInsertJobService robotAccountInsertJobService;

        public ClassificationJobController(
            IProblemAccountJobService problemAccountInsertJobService,
            INormalAccountJobService normalAccountInsertJobService,
            IDailyClassificationJobService dailyClassificationJobService,
            IRealtimeClassificationJobService realtimeClassificationJobService,
            IProbationClassificationJobService probationClassificationJobService,
            IRobotAccountInsertJobService robotAccountInsertJobService)
        {
            this.problemAccountInsertJobService = problemAccountInsertJobService;
            this.normalAccountInsertJobService = normalAccountInsertJobService;
            this.dailyClassificationJobService = dailyClassificationJobService;
            this.realtimeClassificationJobService = realtimeClassificationJobService;
            this.probationClassificationJobService = probationClassificationJobService;
            this.robotAccountInsertJobService = robotAccountInsertJobService;
        }

        [HttpPost("problemAccountInsert")]
        public async Task<bool> InsertProblemAccount([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.problemAccountInsertJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("insertNormalAccount")]
        public async Task<bool> InsertNormalAccount([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountInsertJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("scanningDailyClassification")]
        public async Task<bool> DailyScanningClassification([FromForm] int batchSize)
        {
            return await this.dailyClassificationJobService.Execute(batchSize);
        }

        [HttpPost("scanningRealtimeClassification")]
        public async Task<bool> RealtimeScanningClassification([FromForm] int batchSize)
        {
            return await this.realtimeClassificationJobService.Execute(batchSize);
        }

        [HttpPost("probationClassification")]
        public async Task<bool> ProbationScanningClassification([FromForm] int batchSize)
        {
            return await this.probationClassificationJobService.Execute(batchSize);
        }

        [HttpPost("robotClassification")]
        public async Task<bool> RobotScanningClassification([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.robotAccountInsertJobService.Execute(batchExternalSize, batchInternalSize);
        }
    }
}