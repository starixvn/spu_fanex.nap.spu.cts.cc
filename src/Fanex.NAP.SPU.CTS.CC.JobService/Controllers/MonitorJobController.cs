namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/MonitorJob")]
    public class MonitorJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly ICTSCustomerJobService  ctsCustomerJobService;
        private readonly IClassificationJobMonitorService classificationJobMonitorService;

        public MonitorJobController(ISentryClient sentryClient,
            ICTSCustomerJobService ctsCustomerJobService,
            IClassificationJobMonitorService classificationJobMonitorService)
        {
            this.sentryClient = sentryClient;
            this.ctsCustomerJobService = ctsCustomerJobService;
            this.classificationJobMonitorService = classificationJobMonitorService;
        }

        [HttpPost("MonitorMissingCustomer")]
        public async Task<bool> MonitorMissingCustomer([FromForm] int batchSize)
        {
            return await ctsCustomerJobService.MonitorMissingCustomer(batchSize);
        }

        [HttpPost("MonitorRealtimeInactiveCategory")]
        public async Task<bool> MonitorRealtimeInactiveCategory()
        {
            return await classificationJobMonitorService.MonitorRealtimeInactiveCategory();
        }
    }
}