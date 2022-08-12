namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/updateExternalDataJobs")]
    public class UpdateExternalDataJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly ISabaDangerLevelJobService sabaDangerLevelJobService;

        public UpdateExternalDataJobController(ISentryClient sentryClient,
            ISabaDangerLevelJobService sabaDangerLevelJobService)
        {
            this.sentryClient = sentryClient;
            this.sabaDangerLevelJobService = sabaDangerLevelJobService;
        }

        [HttpPost("sabaDangerLevel")]
        public async Task<bool> UpdateSabaDangerLevel()
        {
            return await this.sabaDangerLevelJobService.Execute();
        }

        [HttpPost("initSabaDangerLevel")]
        public async Task<bool> InitSabaDangerLevel()
        {
            return await this.sabaDangerLevelJobService.InitializeData();
        }
    }
}