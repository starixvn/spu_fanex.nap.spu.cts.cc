namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
<<<<<<< .mine
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
=======
    using System.Threading.Tasks;
    using Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;

    [ApiController]
>>>>>>> .theirs
    [Route("api/twClassificationJob")]
<<<<<<< .mine
    public class TWClassificationJobController : ControllerBase
=======
    public class TWClassificationJobController : ControllerBase
>>>>>>> .theirs
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
