namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/JobMonitoring")]
    public class JobMonitoringController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IIncorrectDataMonitoringJobService incorrectDataNotificationJobService;
        private readonly IClassificationJobMonitorJobService classificationJobMonitorJobService;
        private readonly ICustomerJobMonitorJobService customerJobMonitorService;

        public JobMonitoringController(ISentryClient sentryClient,
            IIncorrectDataMonitoringJobService incorrectDataNotificationJobService,
            IClassificationJobMonitorJobService classificationJobMonitorService,
            ICustomerJobMonitorJobService customerJobMonitorService)
        {
            this.sentryClient = sentryClient;
            this.incorrectDataNotificationJobService = incorrectDataNotificationJobService;
            this.classificationJobMonitorJobService = classificationJobMonitorService;
            this.customerJobMonitorService = customerJobMonitorService;
        }

        [HttpPost("NewCustomerMonitoring")]
        public async Task<bool> MonitorNewCustomer([FromForm] int batchSize, [FromForm] int externalSize)
        {
            return await customerJobMonitorService.ScanMissingCustomer(batchSize, externalSize);
        }

        [HttpPost("CustomerInfoMonitoring")]
        public async Task<bool> MonitorCustomerInfo([FromForm] int batchSize)
        {
            return await customerJobMonitorService.ScanIncorrectCustomerInfo(batchSize);
        }

        [HttpPost("ReatimeCategoryMonitoring")]
        public async Task<bool> MonitorRealtimeCategory([FromForm] int batchSize)
        {
            return await classificationJobMonitorJobService.MonitorRealtimeInactiveCategory(batchSize);
        }

        [HttpPost("NormalClassificationPoolMonitoring")]
        public async Task<bool> MonitorNormalClassificationPool([FromForm] int threshold)
        {
            return await classificationJobMonitorJobService.MonitorNormalClassificationPool(threshold);
        }

        [HttpPost("IncorrectDataMonitoringNotification")]
        public async Task<bool> NotifyIncorrectDataMonitoring()
        {
            return await incorrectDataNotificationJobService.NotififyIncorrectDataMonitoring();
        }

        [HttpGet("health")]
        public string CheckHealth()
        {
            sentryClient.CaptureMessage($"Check health CTS.CC services {DateTime.Now}");
            return JsonConvert.SerializeObject(DateTime.Now);
        }
    }
}