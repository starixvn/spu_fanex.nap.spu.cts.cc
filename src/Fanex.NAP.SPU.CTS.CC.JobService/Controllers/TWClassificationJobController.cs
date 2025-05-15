namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/twClassificationJob")]
    public class TWClassificationJobController : ControllerBase
    {
        private readonly ISentryClient _sentryClient;
        private readonly ITWClassificationJobService _twClassificationJobService;

        public TWClassificationJobController(ISentryClient sentryClient,
            ITWClassificationJobService twClassificationJobService) 
        {
            _sentryClient = sentryClient;
            _twClassificationJobService = twClassificationJobService;
        }

        [HttpPost("ScanTWSpecialLicSubCC")]
        public async Task<bool> ScanTWSpecialLicSubCC([FromForm] int batchInternalSize)
        {
            return await _twClassificationJobService.ScanTWSpecialLicSubCC(batchInternalSize);
        }

        [HttpPost("TWResponseHaifaService")]
        public async Task<bool> TWResponseHaifaService()
        {
            return await _twClassificationJobService.TWResponseHaifaService();
        }

    }
}
