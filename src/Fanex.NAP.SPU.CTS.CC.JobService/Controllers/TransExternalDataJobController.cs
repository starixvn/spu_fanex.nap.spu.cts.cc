namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/transExternalDataJobs")]
    public class TransExternalDataJobController : ControllerBase
    {
        private readonly ISabaDangerLevelJobService sabaDangerLevelJobService;

        public TransExternalDataJobController(
            ISabaDangerLevelJobService sabaDangerLevelJobService)
        {
            this.sabaDangerLevelJobService = sabaDangerLevelJobService;
        }

        [HttpPost("sabaDangerLevel")]
        public async Task<bool> UpdateSabaDangerLevel([FromForm] int batchSize)
        {
            return await this.sabaDangerLevelJobService.Execute(batchSize);
        }

        [HttpPost("initSabaDangerLevel")]
        public async Task<bool> InitSabaDangerLevel([FromForm] int batchSize)
        {
            return await this.sabaDangerLevelJobService.InitializeData(batchSize);
        }
    }
}