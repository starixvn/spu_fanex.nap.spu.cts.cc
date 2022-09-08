namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
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

        public ClassificationBySportJobController(ISentryClient sentryClient,
            INormalAccountBySportJobService normalAccountBySportJobService)
        {
            this.sentryClient = sentryClient;
            this.normalAccountBySportJobService = normalAccountBySportJobService;
        }

        [HttpPost("InsertNormalAccountBySport")]
        public async Task<bool> InsertNormalAccountBySport([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountBySportJobService.Execute(batchExternalSize, batchInternalSize);
        }

    }
}
