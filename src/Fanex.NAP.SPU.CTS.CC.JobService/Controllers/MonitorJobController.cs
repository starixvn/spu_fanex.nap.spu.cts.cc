namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/MonitorJob")]
    public class MonitorJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IClassificationJobMonitorJobService classificationJobMonitorJobService;
        private readonly ICustomerJobMonitorJobService customerJobMonitorService;

        public MonitorJobController(ISentryClient sentryClient,
            IClassificationJobMonitorJobService classificationJobMonitorService,
            ICustomerJobMonitorJobService customerJobMonitorService)
        {
            this.sentryClient = sentryClient;
            this.classificationJobMonitorJobService = classificationJobMonitorService;
            this.customerJobMonitorService = customerJobMonitorService;
        }

        [HttpPost("MonitorNewCustomer")]
        public async Task<bool> MonitorNewCustomer([FromForm] int batchSize)
        {
            return await customerJobMonitorService.Process(batchSize);
        }

        [HttpPost("MonitorRealtimeInactiveCategory")]
        public async Task<bool> MonitorRealtimeInactiveCategory([FromForm] int batchSize)
        {
            return await classificationJobMonitorJobService.MonitorRealtimeInactiveCategory(batchSize);
        }
    }
}