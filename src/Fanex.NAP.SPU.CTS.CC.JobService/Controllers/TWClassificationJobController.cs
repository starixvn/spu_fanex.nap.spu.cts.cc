namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using System.Threading.Tasks;
    using Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;

    [ApiController]
    [Route("api/twclassification")]
    public class TWClassificationJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly ITWClassificationJobService twClassificationJobService;

        public TWClassificationJobController(ISentryClient sentryClient,
            ITWClassificationJobService twClassificationJobService)
        {
            this.sentryClient = sentryClient;
            this.twClassificationJobService = twClassificationJobService;
        }

        [HttpPost("TWResponseHaifaService")]
        public async Task<bool> TWResponseHaifaService()
        {
            return await this.twClassificationJobService.TWResponseHaifaService();
        }
    }
}
