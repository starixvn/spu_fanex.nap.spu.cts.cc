namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Fanex.NAP.SPU.CTS.Shares;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/JobMonitoring")]
    public class JobMonitoringController : ControllerBase
    {
        private readonly IIncorrectDataMonitoringJobService incorrectDataNotificationJobService;
        private readonly IClassificationJobMonitorJobService classificationJobMonitorJobService;
        private readonly ICustomerJobMonitorJobService customerJobMonitorService;

        public JobMonitoringController(
            IIncorrectDataMonitoringJobService incorrectDataNotificationJobService,
            IClassificationJobMonitorJobService classificationJobMonitorService,
            ICustomerJobMonitorJobService customerJobMonitorService)
        {
            this.incorrectDataNotificationJobService = incorrectDataNotificationJobService;
            this.classificationJobMonitorJobService = classificationJobMonitorService;
            this.customerJobMonitorService = customerJobMonitorService;
        }

        [HttpPost("NewCustomerMonitoring")]
        public async Task<bool> MonitorNewCustomer([FromForm] int internalBatchSize, [FromForm] int externalBatchSize, [FromForm] int addSecond)
        {
            return await customerJobMonitorService.ScanMissingCustomer(internalBatchSize, externalBatchSize, addSecond);
        }

        [HttpPost("ReactivatedCustomerMonitoring")]
        public async Task<bool> MonitorReactivatedCustomer([FromForm] int batchSize, [FromForm] int addSecond)
        {
            return await customerJobMonitorService.ScanMissingReactivatedCustomer(batchSize, addSecond);
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

        [HttpPost("NormalPoolMonitoring")]
        public async Task<bool> MonitorNormalPools([FromForm] int threshold)
        {
            return await classificationJobMonitorJobService.MonitorNormalPools(threshold);
        }

        [HttpPost("IncorrectDataMonitoringNotification")]
        public async Task<bool> NotifyIncorrectDataMonitoring()
        {
            return await incorrectDataNotificationJobService.NotififyIncorrectDataMonitoring();
        }

        [HttpGet("health")]
        public string CheckHealth()
        {
            LogExtension.LogInfo($"Check health CTS.CC services {DateTime.Now}");
            return JsonConvert.SerializeObject(DateTime.Now);
        }
    }
}