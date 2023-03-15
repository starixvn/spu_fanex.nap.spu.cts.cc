namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Microsoft.AspNetCore.Mvc;   
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/twrobotImperva")]
    public class TWRobotImpervaJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;

        public TWRobotImpervaJobController()
        {

        }

    }
}
