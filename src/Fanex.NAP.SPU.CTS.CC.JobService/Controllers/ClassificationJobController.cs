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
        private readonly IDailyProblemClassificationJobService dailyProblemClassificationServices;

        private readonly IRealtimeCheckChangeClassificationJobService realtimeCheckChangeJobService;
        private readonly IRealtimeClassificationJobService realtimeJobService;
        private readonly INormalAccountClassificationJobService normalAccountClassificationJobService;
        private readonly INormalAccountJobService normalAccountJobService;
        private readonly IProbationClassificationJobService probationClassificationJobService;
        private readonly IDailyNormalClassificationJobService dailyNormalClassificationJobService;
        private readonly INewCustomerClassificationJobService newCustomerClassificationJobService;
        private readonly IAffectedDownlineCustomerClassificationJobService affectedDownlineClassificationJobService;
        private readonly INormalTaggingClassificationJobService normalTaggingClassificationJobService;

        private readonly IRobotAccountInsertJobService robotAccountInsertJobService;
        private readonly ITWProblemAccountJobService twProblemAccountJobService;
        private readonly ITVSProblemAccountJobService tvsProblemAccountJobService;
        private readonly IMatchMonitorClassificationJobService mmClassificationJobService;       
        private readonly IDetectUnauthorizedLoginCustomerJobService detectUnauthorizedLoginCustJobService;
        private readonly IDailyDangerousClassificationJobService dailyDangerousClassificationJobService;
        private readonly IRealtimeTWInitialGroupBettingJobService realtimeTWInitialGroupBettingJobService;
        private readonly IRealtimeTWInitialSmartJobService realtimeTWInitialSmartJobService;

        public ClassificationJobController(ISentryClient sentryClient,           
            IDailyProblemClassificationJobService dailyProblemClassificationServices,
            IRealtimeCheckChangeClassificationJobService realtimeCheckChangeJobService,
            IRealtimeClassificationJobService realtimeJobService,
            INormalAccountClassificationJobService normalAccountClassificationJobService,
            INormalAccountJobService normalAccountJobService,
            IProbationClassificationJobService probationClassificationJobService,
            IDailyNormalClassificationJobService dailyNormalClassificationJobService,
            INewCustomerClassificationJobService newCustomerClassificationJobService,
            IAffectedDownlineCustomerClassificationJobService affectedDownlineClassificationJobService,
            INormalTaggingClassificationJobService normalTaggingClassificationJobService,

            IRobotAccountInsertJobService robotAccountInsertJobService,
            ITWProblemAccountJobService twProblemAccountJobService,
            ITVSProblemAccountJobService tvsProblemAccountJobService,
            IMatchMonitorClassificationJobService mmClassificationJobService,            
            IDetectUnauthorizedLoginCustomerJobService detectUnauthorizedLoginCustJobService,
            IDailyDangerousClassificationJobService dailyDangerousClassificationJobService,
            IRealtimeTWInitialGroupBettingJobService realtimeTWInitialGroupBettingJobService,
            IRealtimeTWInitialSmartJobService realtimeTWInitialSmartJobService)
        {
            this.sentryClient = sentryClient;           
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;                       

            this.realtimeCheckChangeJobService = realtimeCheckChangeJobService;
            this.realtimeJobService = realtimeJobService;
            this.normalAccountClassificationJobService = normalAccountClassificationJobService;
            this.normalAccountJobService = normalAccountJobService;
            this.probationClassificationJobService = probationClassificationJobService;
            this.dailyNormalClassificationJobService = dailyNormalClassificationJobService;
            this.newCustomerClassificationJobService = newCustomerClassificationJobService;
            this.affectedDownlineClassificationJobService = affectedDownlineClassificationJobService;
            this.normalTaggingClassificationJobService = normalTaggingClassificationJobService;

            this.robotAccountInsertJobService = robotAccountInsertJobService;
            this.twProblemAccountJobService = twProblemAccountJobService;
            this.tvsProblemAccountJobService = tvsProblemAccountJobService;
            this.mmClassificationJobService = mmClassificationJobService;          
            this.detectUnauthorizedLoginCustJobService = detectUnauthorizedLoginCustJobService;
            this.dailyDangerousClassificationJobService = dailyDangerousClassificationJobService;
            this.realtimeTWInitialGroupBettingJobService = realtimeTWInitialGroupBettingJobService;
            this.realtimeTWInitialSmartJobService = realtimeTWInitialSmartJobService;
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

 		[HttpPost("scanningDailyDangerousDepositClassification")]
        public async Task<bool> DailyScanningDangerousDepositClassification([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Daily Deposit Dangerous Score Classification Service is running at {DateTime.Now}");

            return await this.dailyDangerousClassificationJobService.Execute(batchSize, true);
        }

        [HttpPost("scanningDailyDangerousCreditClassification")]
        public async Task<bool> DailyScanningDangerousCreditClassification([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Daily Credit Dangerous Score Classification Service is running at {DateTime.Now}");

            return await this.dailyDangerousClassificationJobService.Execute(batchSize, false);
        }
      
        [HttpPost("realtimeCheckChanges")]
        public async Task<bool> RealtimeCheckChanges([FromForm] int batchSize)
        {
            return await this.realtimeCheckChangeJobService.Execute(batchSize);           
        }

        [HttpPost("scanningRealtimeClassification")]
        public async Task<bool> RealtimeScanningClassification([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.realtimeJobService.Execute(batchExternalSize, batchInternalSize);            
        }

        [HttpPost("normalAccountClassification")]
        public async Task<bool> NormalAccountClassification([FromForm] int batchInternalSize)
        {
            return await this.normalAccountClassificationJobService.Execute(batchInternalSize);
        }

        [HttpPost("normalAccountInsert")]
        public async Task<bool> NormalAccountInsert([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("probationClassification")]
        public async Task<bool> ProbationScanningClassification([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Probation Service is running at {DateTime.Now}");
            return await this.probationClassificationJobService.Execute(batchSize);
        }

        [HttpPost("scanningDailyNormalTaggingClassification")]
        public async Task<bool> DailyScanningNormalTaggingClassification([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Daily Normal Tagging Service is running at {DateTime.Now}");
            return await this.normalTaggingClassificationJobService.Execute(batchSize);
        }

        [HttpPost("newcustomerclassification")]
        public async Task<bool> NewCustomerClassification([FromForm] int batchSize)
        {
            return await newCustomerClassificationJobService.Execute(batchSize);
        }

        [HttpPost("affectedDownlineClassificationFromQueue")]
        public async Task<bool> AffectedDownlineClassificationFromQueue([FromForm] int batchSize)
        {
            return await this.affectedDownlineClassificationJobService.Execute(batchSize);
        }      

        [HttpPost("robotTWClassification")]
        public async Task<bool> RobotTWClassication([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.robotAccountInsertJobService.ScanningTWRobotAccount(batchExternalSize, batchInternalSize);
        }

        [HttpPost("robotAIImpervaDetection")]
        public async Task<bool> RobotAIImpervaDetection([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {           
            return await this.robotAccountInsertJobService.ScanningAIImpervaRobotClassification(batchExternalSize, batchInternalSize);
        }

        [HttpPost("scan-tw-problem-account-customer")]
        public async Task<bool> ScanTWSystemFormulaBetCustomer()
        {
            return await twProblemAccountJobService.Execute();
        }       

        [HttpPost("tvsProblemAccountInsert")]
        public async Task<bool> TVSProblemAccountInsert([FromForm] int batchSize)
        {
            return await tvsProblemAccountJobService.Execute(batchSize);
        }

        [HttpPost("DetectUnauthorizedLoginCustomerCredit")]
        public async Task<bool> DetectUnauthorizedLoginCustomerCredit([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await detectUnauthorizedLoginCustJobService.Execute(batchExternalSize, batchInternalSize, false);
        }

        [HttpPost("DetectUnauthorizedLoginCustomerLicensee")]
        public async Task<bool> DetectUnauthorizedLoginCustomerLicensee([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await detectUnauthorizedLoginCustJobService.Execute(batchExternalSize, batchInternalSize, true);
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

        [HttpPost("scanningSecondlyTWInitialGBClassification")]
        public async Task<bool> RealtimeTWInitialGBClassification([FromForm] int batchSize)
        {            
            return await this.realtimeTWInitialGroupBettingJobService.Execute(batchSize);
        }

        [HttpPost("scanningMinutelyTWInitialSmartClassification")]
        public async Task<bool> RealtimeTWInitialSmartClassification([FromForm] int batchSize)
        {
            return await this.realtimeTWInitialSmartJobService.Execute(batchSize);
        }
    }
}