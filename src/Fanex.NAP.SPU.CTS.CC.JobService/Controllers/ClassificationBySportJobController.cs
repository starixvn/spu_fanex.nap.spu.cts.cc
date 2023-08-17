namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;
    using Renovate = Services.CustomerClassificationBySport.Renovate;

    [ApiController]
    [Route("api/classificationBySportJobs")]
    public class ClassificationBySportJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;

        private readonly Renovate.INormalAccountClassificationBySportJobService normalAccountClassificationBySportRenovateJobService;
        private readonly Renovate.INormalAccountInsertBySportJobService normalAccountInsertBySportRenovateJobService;
        private readonly Renovate.IRealtimeClassificationBySportJobService realtimeClassificationBySportRenovateJobService;
        private readonly Renovate.IProbationClassificationBySportJobService probationClassificationBySportRenovateJobService;
        private readonly Renovate.IDailyNormalClassificationBySportJobService dailyNormalClassificationBySportRenovateJobService;

        public ClassificationBySportJobController(ISentryClient sentryClient,
            Renovate.INormalAccountClassificationBySportJobService normalAccountClassificationBySportRenovateJobService,
            Renovate.INormalAccountInsertBySportJobService normalAccountInsertBySportRenovateJobService,
            Renovate.IRealtimeClassificationBySportJobService realtimeClassificationBySportRenovateJobService,
            Renovate.IProbationClassificationBySportJobService probationClassificationBySportRenovateJobService,
            Renovate.IDailyNormalClassificationBySportJobService dailyNormalClassificationBySportRenovateJobService)
        {
            this.sentryClient = sentryClient;
            this.normalAccountClassificationBySportRenovateJobService = normalAccountClassificationBySportRenovateJobService;
            this.normalAccountInsertBySportRenovateJobService = normalAccountInsertBySportRenovateJobService;
            this.realtimeClassificationBySportRenovateJobService = realtimeClassificationBySportRenovateJobService;
            this.probationClassificationBySportRenovateJobService = probationClassificationBySportRenovateJobService;
            this.dailyNormalClassificationBySportRenovateJobService = dailyNormalClassificationBySportRenovateJobService;
        }

        #region Renovate
        [HttpPost("NormalAccountClassificationBySportRenovate")]
        public async Task<bool> NormalAccountClassificationBySportRenovate([FromForm] int batchInternalSize)
        {
            return await this.normalAccountClassificationBySportRenovateJobService.Execute(batchInternalSize);
        }

        [HttpPost("NormalAccountInsertBySportRenovate")]
        public async Task<bool> NormalAccountInsertBySportRenovate([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountInsertBySportRenovateJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("ScanRealtimeClassificationBySportRenovate")]
        public async Task<bool> ScanRealtimeClassificationBySportRenovate([FromForm] int batchInternalSize)
        {
            return await this.realtimeClassificationBySportRenovateJobService.Execute(batchInternalSize);
        }

        [HttpPost("ScanProbationClassificationBySportRenovate")]
        public async Task<bool> ScanProbationClassificationBySportRenovate([FromForm] int batchSize)
        {
            //sentryClient.CaptureMessage($"ScanProbationClassificationBySportRenovate Service is running at {DateTime.Now}");

            return await this.probationClassificationBySportRenovateJobService.Execute(batchSize);
        }


        [HttpPost("ScanningDailyNormalClassificationBySportRenovate")]
        public async Task<bool> ScanningDailyNormalClassificationBySportRenovate([FromForm] int batchSize)
        {
            //sentryClient.CaptureMessage($"ScanningDailyNormalClassificationBySportRenovate Service is running at {DateTime.Now}");

            return await this.dailyNormalClassificationBySportRenovateJobService.Execute();
        }

        #endregion Renovate

    }
}
