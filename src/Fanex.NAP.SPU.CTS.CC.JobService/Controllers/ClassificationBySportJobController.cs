namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using Renovate = Services.CustomerClassificationBySport.Renovate;

    [ApiController]
    [Route("api/classificationBySportJobs")]
    public class ClassificationBySportJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountBySportJobService normalAccountBySportJobService;
        private readonly IDailyNormalClassificationBySportJobService dailyClassificationBySportServices;
        private readonly IRealtimeClassificationBySportJobService realtimeClassificationJobService;
        private readonly IProbationClassificationBySportJobService probationClassificationBySportJobService;

        private readonly Renovate.INormalAccountClassificationBySportJobService normalAccountClassificationBySportRenovateJobService;
        private readonly Renovate.INormalAccountInsertBySportJobService normalAccountInsertBySportJobService;
        private readonly Renovate.IRealtimeClassificationBySportJobService realtimeClassificationBySportJobService;

        public ClassificationBySportJobController(ISentryClient sentryClient,
            INormalAccountBySportJobService normalAccountBySportJobService,
            IDailyNormalClassificationBySportJobService dailyClassificationBySportServices,
            IRealtimeClassificationBySportJobService realtimeClassificationJobService,
            IProbationClassificationBySportJobService probationClassificationBySportJobService,
            Renovate.INormalAccountClassificationBySportJobService normalAccountClassificationBySportRenovateJobService,
            Renovate.INormalAccountInsertBySportJobService normalAccountInsertBySportJobService,
            Renovate.IRealtimeClassificationBySportJobService realtimeClassificationBySportJobService)
        {
            this.sentryClient = sentryClient;
            this.normalAccountBySportJobService = normalAccountBySportJobService;
            this.dailyClassificationBySportServices = dailyClassificationBySportServices;
            this.realtimeClassificationJobService = realtimeClassificationJobService;
            this.probationClassificationBySportJobService = probationClassificationBySportJobService;

            this.normalAccountClassificationBySportRenovateJobService = normalAccountClassificationBySportRenovateJobService;
            this.normalAccountInsertBySportJobService = normalAccountInsertBySportJobService;
            this.realtimeClassificationBySportJobService = realtimeClassificationBySportJobService;
        }

        [HttpPost("InsertNormalAccountBySport")]
        public async Task<bool> InsertNormalAccountBySport([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountBySportJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("ScanRealtimeClassificationBySport")]
        public async Task<bool> RealtimeScanningClassification([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.realtimeClassificationJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("ScanningDailyNormalClassificationBySport")]
        public async Task<bool> ScanningDailyNormalClassificationBySport()
        {
            //sentryClient.CaptureMessage($"Scanning Daily Normal By Sport Service is running at {DateTime.Now}");

            return await this.dailyClassificationBySportServices.Execute();
        }

        [HttpPost("ScanningProbationClassificationBySport")]
        public async Task<bool> ScanningProbationClassificationBySport([FromForm] int batchSize)
        {
            //sentryClient.CaptureMessage($"Probation By Sport Service is running at {DateTime.Now}");

            return await this.probationClassificationBySportJobService.Execute(batchSize);
        }

        #region Renovate
        [HttpPost("NormalAccountClassificationBySportRenovate")]
        public async Task<bool> NormalAccountClassificationBySportRenovate([FromForm] int batchInternalSize)
        {
            //sentryClient.CaptureMessage($"NormalAccountClassificationBySportRenovate Service is running at {DateTime.Now}");

            return await this.normalAccountClassificationBySportRenovateJobService.Execute(batchInternalSize);
        }

        [HttpPost("NormalAccountInsertBySportRenovate")]
        public async Task<bool> NormalAccountInsertBySportRenovate([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            //sentryClient.CaptureMessage($"NormalAccountInsertBySportRenovate Service is running at {DateTime.Now}");

            return await this.normalAccountInsertBySportJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("ScanRealtimeClassificationBySportRenovate")]
        public async Task<bool> ScanRealtimeClassificationBySportRenovate([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            //sentryClient.CaptureMessage($"ScanRealtimeClassificationBySportRenovate Service is running at {DateTime.Now}");

            return await this.realtimeClassificationBySportJobService.Execute(batchExternalSize, batchInternalSize);
        }

        #endregion Renovate

    }
}
