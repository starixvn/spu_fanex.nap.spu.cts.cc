namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/twClassificationJob")]
    public class TWClassificationJobController : ControllerBase
    {
        private readonly ITWClassificationJobService _twClassificationJobService;

        public TWClassificationJobController(ITWClassificationJobService twClassificationJobService) 
        {
            _twClassificationJobService = twClassificationJobService;
        }

        [HttpPost("ScanTWSpecialLicSubCC")]
        public async Task<bool> ScanTWSpecialLicSubCC([FromForm] int batchInternalSize)
        {
            return await _twClassificationJobService.ScanTWSpecialLicSubCC(batchInternalSize);
        }

        [HttpPost("ResponseTWSpecialLicSubCC")]
        public async Task<bool> ResponseTWSpecialLicSubCC([FromForm] int batchSize, [FromForm] bool isResponse)
        {
            return await _twClassificationJobService.ResponseTWSpecialLicSubCC(batchSize, isResponse);
        }
    }
}
