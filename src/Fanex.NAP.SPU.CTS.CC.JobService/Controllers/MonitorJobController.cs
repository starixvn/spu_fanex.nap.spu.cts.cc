namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Fanex.NAP.SPU.CTS.Shares;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Sentry;
    using System;
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
        public async Task<bool> MonitorNewCustomer([FromForm] int batchSize, [FromForm] int externalSize)
        {
            return await customerJobMonitorService.Process(batchSize, externalSize);
        }

        [HttpPost("MonitorRealtimeInactiveCategory")]
        public async Task<bool> MonitorRealtimeInactiveCategory([FromForm] int batchSize)
        {
            return await classificationJobMonitorJobService.MonitorRealtimeInactiveCategory(batchSize);
        }

        [HttpGet("health")]
        public string CheckHealth()
        {
            sentryClient.CaptureMessage($"Check health CTS.CC services {DateTime.Now}", Sentry.Protocol.SentryLevel.Info);
            return JsonConvert.SerializeObject(DateTime.Now.ToString());
        }
    }
}