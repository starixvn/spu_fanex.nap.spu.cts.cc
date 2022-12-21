namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.AssociatedGroupMonitor;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/associationJob")]
    public class AssociationJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IAssociatedGroupMonitorJobService associatedGroupMonitorJobService;

        public AssociationJobController(ISentryClient sentryClient, IAssociatedGroupMonitorJobService associatedGroupMonitorJobService)
        {
            this.sentryClient = sentryClient;            
            this.associatedGroupMonitorJobService = associatedGroupMonitorJobService;
        }

        [HttpPost("ScanAssociatedAccount")]
        public async Task<bool> ScanAssociatedAccount()
        {
            return await this.associatedGroupMonitorJobService.ScanAssociatedAccount();
        }
    }
}
