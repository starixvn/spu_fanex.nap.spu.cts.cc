namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/evidenceJobs")]
    public class EvidencesJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IRemoveAccountEvidenceJobService removeAccountEvidenceJobService;
        private readonly ICustomerEvidenceInsertJobService customerEvidenceInsertJobService;

        public EvidencesJobController(ISentryClient sentryClient,
            IRemoveAccountEvidenceJobService removeAccountEvidenceJobService,
            ICustomerEvidenceInsertJobService customerEvidenceInsertJobService)
        {
            this.sentryClient = sentryClient;
            this.removeAccountEvidenceJobService = removeAccountEvidenceJobService;
            this.customerEvidenceInsertJobService = customerEvidenceInsertJobService;
        }

        [HttpPost("removeAccountEvidence")]
        public async Task<bool> RemoveAccountEvidences([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.removeAccountEvidenceJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("insertAccountEvidence")]
        public async Task<bool> CustomerAccountEvidences([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.customerEvidenceInsertJobService.Execute(batchExternalSize, batchInternalSize);
        }
    }
}