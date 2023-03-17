namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/classificationJobs")]
    public class ClassificationJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountJobService normalAccountInsertJobService;
        private readonly IDailyNormalClassificationJobService dailyNormalClassificationJobService;
        private readonly IDailyProblemClassificationJobService dailyProblemClassificationServices;
        private readonly IRealtimeClassificationJobService realtimeClassificationJobService;
        private readonly IProbationClassificationJobService probationClassificationJobService;
        private readonly IRobotAccountInsertJobService robotAccountInsertJobService;
        private readonly ITWProblemAccountJobService twProblemAccountJobService;
        private readonly ITVSProblemAccountJobService tvsProblemAccountJobService;
        private readonly INewCustomerClassificationJobService newCustomerClassificationJobService;
        private readonly IAffectedDownlineCustomerClassificationJobService affectedDownlineCustomerClassificationJobService;
        private readonly IMatchMonitorClassificationJobService mmClassificationJobService;
        private readonly ITWRobotImpervaJobService robotImpervaJobService;

        public ClassificationJobController(ISentryClient sentryClient,
            INormalAccountJobService normalAccountInsertJobService,
            IDailyNormalClassificationJobService dailyNormalClassificationJobService,
            IDailyProblemClassificationJobService dailyProblemClassificationServices,
            IRealtimeClassificationJobService realtimeClassificationJobService,
            IProbationClassificationJobService probationClassificationJobService,
            IRobotAccountInsertJobService robotAccountInsertJobService,
            ITWProblemAccountJobService twProblemAccountJobService,
            ITVSProblemAccountJobService tvsProblemAccountJobService,
            INewCustomerClassificationJobService newCTSCustomerClassificationJobService,
            IAffectedDownlineCustomerClassificationJobService affectedDownlineCustomerClassificationJobService,
            IMatchMonitorClassificationJobService mmClassificationJobService,
            ITWRobotImpervaJobService robotImpervaJobService)
        {
            this.sentryClient = sentryClient;
            this.normalAccountInsertJobService = normalAccountInsertJobService;
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;
            this.dailyNormalClassificationJobService = dailyNormalClassificationJobService;
            this.realtimeClassificationJobService = realtimeClassificationJobService;
            this.probationClassificationJobService = probationClassificationJobService;
            this.robotAccountInsertJobService = robotAccountInsertJobService;
            this.twProblemAccountJobService = twProblemAccountJobService;
            this.tvsProblemAccountJobService = tvsProblemAccountJobService;
            this.newCustomerClassificationJobService = newCTSCustomerClassificationJobService;
            this.affectedDownlineCustomerClassificationJobService = affectedDownlineCustomerClassificationJobService;
            this.mmClassificationJobService = mmClassificationJobService;
            this.robotImpervaJobService = robotImpervaJobService;
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

        [HttpPost("scanningDailyProblemClassification")]
        public async Task<bool> DailyScanningProblemClassification([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Daily Problem Service is running at {DateTime.Now}");
            return await this.dailyProblemClassificationServices.Execute(batchSize);
        }

        [HttpPost("scanningRealtimeClassification")]
        public async Task<bool> RealtimeScanningClassification([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.realtimeClassificationJobService.ProcessClassification(batchExternalSize, batchInternalSize);
        }

        [HttpPost("checkChangeRealtimeClassification")]
        public async Task<bool> RealtimeCheckChanges()
        {
            return await this.realtimeClassificationJobService.CheckChanges();
        }

        [HttpPost("probationClassification")]
        public async Task<bool> ProbationScanningClassification([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Probation Service is running at {DateTime.Now}");
            return await this.probationClassificationJobService.Execute(batchSize);
        }

        [HttpPost("robotTWClassification")]
        public async Task<bool> RobotTWClassication([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.robotAccountInsertJobService.ScanningTWRobotAccount(batchExternalSize, batchInternalSize);
        }

        [HttpPost("robotAIImpervaDetection")]
        public async Task<bool> RobotAIImpervaDetection([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            Logging.LogService.Info("robotAIImpervaDetection calling");

            return await this.robotAccountInsertJobService.ScanningAIImpervaRobotClassification(batchExternalSize, batchInternalSize);
        }

        [HttpPost("scan-tw-problem-account-customer")]
        public async Task<bool> ScanTWSystemFormulaBetCustomer()
        {
            return await twProblemAccountJobService.Execute();
        }

        [HttpPost("robotImpervaGet")]
        public async Task<bool> RobotImpervaGet([FromForm] int batchSize)
        {
            Logging.LogService.Info("RobotImpervaGet calling");

            return await this.robotImpervaJobService.Execute(batchSize);
        }

        [HttpPost("tvsProblemAccountInsert")]
        public async Task<bool> TVSProblemAccountInsert([FromForm] int batchSize)
        {
            return await tvsProblemAccountJobService.Execute(batchSize);
        }

        [HttpPost("newcustomerclassification")]
        public async Task<bool> NewCustomerClassification([FromForm] int batchSize)
        {
            return await newCustomerClassificationJobService.Execute(batchSize);
        }

        [HttpPost("affectedDownlineClassificationFromQueue")]
        public async Task<bool> AffectedDownlineClassificationFromQueue([FromForm] int batchSize)
        {
            return await this.affectedDownlineCustomerClassificationJobService.Execute(batchSize);
        }

        [HttpPost("UpdateStatusMatchMonitor")]
        public async Task<bool> UpdateStatusMatchMonitor()
        {
            return await this.mmClassificationJobService.UpdateStatusMatchMonitor();
        }

        [HttpPost("ClassifyMatchMonitor")]
        public async Task<bool> ClassifyMatchMonitor()
        {
            return await this.mmClassificationJobService.ClassifyMatchMonitor();
        }

        [HttpPost("updatewritelog")]
        public async Task<bool> UpdateWriteLog([FromForm] string serviceName, [FromForm] bool isWriteLog)
        {
            return await this.normalAccountInsertJobService.UpdateWriteLogSetting(serviceName, isWriteLog);
        }
    }
}