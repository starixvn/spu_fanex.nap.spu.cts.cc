namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.AssociatedGroupMonitor;
    using Microsoft.AspNetCore.Mvc;
    
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/associationJob")]
    public class AssociationJobController : ControllerBase
    {
        private readonly IAssociatedGroupMonitorJobService associatedGroupMonitorJobService;

        public AssociationJobController(IAssociatedGroupMonitorJobService associatedGroupMonitorJobService)
        {
            this.associatedGroupMonitorJobService = associatedGroupMonitorJobService;
        }

        [HttpPost("ScanAssociatedAccount")]
        public async Task<bool> ScanAssociatedAccount()
        {
            return await this.associatedGroupMonitorJobService.ScanAssociatedAccount();
        }
    }
}
