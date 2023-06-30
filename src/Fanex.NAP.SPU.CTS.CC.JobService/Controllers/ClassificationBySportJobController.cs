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

        public ClassificationBySportJobController(ISentryClient sentryClient,
            INormalAccountBySportJobService normalAccountBySportJobService,
            IDailyNormalClassificationBySportJobService dailyClassificationBySportServices,
            IRealtimeClassificationBySportJobService realtimeClassificationJobService,
            IProbationClassificationBySportJobService probationClassificationBySportJobService,
            Renovate.INormalAccountClassificationBySportJobService normalAccountClassificationBySportRenovateJobService)
        {
            this.sentryClient = sentryClient;
            this.normalAccountBySportJobService = normalAccountBySportJobService;
            this.dailyClassificationBySportServices = dailyClassificationBySportServices;
            this.realtimeClassificationJobService = realtimeClassificationJobService;
            this.probationClassificationBySportJobService = probationClassificationBySportJobService;

            this.normalAccountClassificationBySportRenovateJobService = normalAccountClassificationBySportRenovateJobService;
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
        [HttpPost("normalAccountClassificationBySportRenovate")]
        public async Task<bool> NormalAccountClassificationBySportRenovate([FromForm] int batchInternalSize)
        {
            return await this.normalAccountClassificationBySportRenovateJobService.Execute(batchInternalSize);
        }

        #endregion Renovate

    }
}
