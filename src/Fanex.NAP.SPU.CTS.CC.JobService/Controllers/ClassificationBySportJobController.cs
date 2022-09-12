namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/classificationBySportJobs")]
    public class ClassificationBySportJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountBySportJobService normalAccountBySportJobService;
        private readonly IDailyNormalClassificationBySportJobService dailyClassificationBySportServices;

        public ClassificationBySportJobController(ISentryClient sentryClient
            , INormalAccountBySportJobService normalAccountBySportJobService
            , IDailyNormalClassificationBySportJobService dailyClassificationBySportServices)
        {
            this.sentryClient = sentryClient;
            this.normalAccountBySportJobService = normalAccountBySportJobService;
            this.dailyClassificationBySportServices = dailyClassificationBySportServices;
        }

        [HttpPost("InsertNormalAccountBySport")]
        public async Task<bool> InsertNormalAccountBySport([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountBySportJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("ScanningDailyNormalClassificationBySport")]
        public async Task<bool> ScanningDailyNormalClassificationBySport()
        {
            //sentryClient.CaptureMessage($"Scanning Daily Normal By Sport Service is running at {DateTime.Now}");

            return await this.dailyClassificationBySportServices.Execute();
        }
    }
}
