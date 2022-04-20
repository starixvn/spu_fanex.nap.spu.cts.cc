namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/classificationJobs")]
    public class ClassificationJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IProblemAccountJobService problemAccountInsertJobService;
        private readonly INormalAccountJobService normalAccountInsertJobService;
        private readonly IDailyNormalClassificationJobService dailyNormalClassificationJobService;
        private readonly IDailyProblemClassificationJobService dailyProblemClassificationServices;
        private readonly IRealtimeClassificationJobService realtimeClassificationJobService;
        private readonly IProbationClassificationJobService probationClassificationJobService;
        private readonly IRobotAccountInsertJobService robotAccountInsertJobService;
        private readonly ICustomerCategoryJobService customerCategoryJobService;

        public ClassificationJobController(ISentryClient sentryClient,
            IProblemAccountJobService problemAccountInsertJobService,
            INormalAccountJobService normalAccountInsertJobService,
            IDailyNormalClassificationJobService dailyNormalClassificationJobService,
            IDailyProblemClassificationJobService dailyProblemClassificationServices,
            IRealtimeClassificationJobService realtimeClassificationJobService,
            IProbationClassificationJobService probationClassificationJobService,
            IRobotAccountInsertJobService robotAccountInsertJobService,
            ICustomerCategoryJobService customerCategoryJobService)
        {
            this.sentryClient = sentryClient;
            this.problemAccountInsertJobService = problemAccountInsertJobService;
            this.normalAccountInsertJobService = normalAccountInsertJobService;
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;
            this.dailyNormalClassificationJobService = dailyNormalClassificationJobService;
            this.realtimeClassificationJobService = realtimeClassificationJobService;
            this.probationClassificationJobService = probationClassificationJobService;
            this.robotAccountInsertJobService = robotAccountInsertJobService;
            this.customerCategoryJobService = customerCategoryJobService;
        }

        [HttpPost("problemAccountInsert")]
        public async Task<bool> InsertProblemAccount([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.problemAccountInsertJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("insertNormalAccount")]
        public async Task<bool> InsertNormalAccount([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountInsertJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("scanningDailyNormalClassification")]
        public async Task<bool> DailyScanningNormalClassification()
        {
            sentryClient.CaptureMessage($"Daily Normal Service is running at {DateTime.Now}");
            return await this.dailyNormalClassificationJobService.Execute();
        }

        [HttpPost("scanningDailyProblemProbationClassification")]
        public async Task<bool> DailyScanningProblemProbationClassification([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Daily Problem Probation Service is running at {DateTime.Now}");
            return await this.dailyProblemClassificationServices.Execute(batchSize);
        }

        [HttpPost("scanningRealtimeClassification")]
        public async Task<bool> RealtimeScanningClassification([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.realtimeClassificationJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("probationClassification")]
        public async Task<bool> ProbationScanningClassification([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Probation Service is running at {DateTime.Now}");
            return await this.probationClassificationJobService.Execute(batchSize);
        }

        [HttpPost("robotClassification")]
        public async Task<bool> RobotScanningClassification([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.robotAccountInsertJobService.ScanningRobotClassification(batchExternalSize, batchInternalSize);
        }

        [HttpPost("robotAIClassification")]
        public async Task<bool> RobotAIScanningClassification([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.robotAccountInsertJobService.ScanningRobotAIClassification(batchExternalSize, batchInternalSize);
        }

        [HttpPost("InsertCustCategoryVVIPFromQueue")]
        public async Task<bool> InsertCustCategoryVVIPFromQueue()
        {
            return await this.customerCategoryJobService.Execute();
        }

        [HttpPost("updatewritelog")]
        public async Task<bool> UpdateWriteLog([FromForm] string serviceName, [FromForm] bool isWriteLog)
        {
            return await this.normalAccountInsertJobService.UpdateWriteLogSetting(serviceName, isWriteLog);
        }
        
    }
}