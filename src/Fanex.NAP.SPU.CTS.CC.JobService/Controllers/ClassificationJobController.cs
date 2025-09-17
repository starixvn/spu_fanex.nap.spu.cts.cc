namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.NormalDangerousAssociation;
    using Fanex.NAP.SPU.CTS.Shares;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/classificationJobs")]
    public class ClassificationJobController : ControllerBase
    {
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
        private readonly INormalDangerousAssociationCheckJobService normalDangerousAssociationCheckJobService;
        private readonly INormalDangerousAssociationScanJobService normalDangerousAssociationScanJobService;

        private readonly IRobotAccountInsertJobService robotAccountInsertJobService;
        private readonly ITVSProblemAccountJobService tvsProblemAccountJobService;
        private readonly IMatchMonitorClassificationJobService mmClassificationJobService;
        private readonly IDetectUnauthorizedLoginCustomerJobService detectUnauthorizedLoginCustJobService;
        private readonly IDailyDangerousClassificationJobService dailyDangerousClassificationJobService;
        private readonly IRealtimeTWInitialGroupBettingJobService realtimeTWInitialGroupBettingJobService;
        private readonly IRealtimeTWInitialSmartJobService realtimeTWInitialSmartJobService;
        private readonly IRealtimeTWInitialSmartBySportJobService realtimeTWInitialSmartBySportJobService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "<Pending>")]
        public ClassificationJobController(

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
            INormalDangerousAssociationCheckJobService normalDangerousAssociationCheckJobService,
            INormalDangerousAssociationScanJobService normalDangerousAssociationScanJobService,

            IRobotAccountInsertJobService robotAccountInsertJobService,
            ITVSProblemAccountJobService tvsProblemAccountJobService,
            IMatchMonitorClassificationJobService mmClassificationJobService,
            IDetectUnauthorizedLoginCustomerJobService detectUnauthorizedLoginCustJobService,
            IDailyDangerousClassificationJobService dailyDangerousClassificationJobService,
            IRealtimeTWInitialGroupBettingJobService realtimeTWInitialGroupBettingJobService,
            IRealtimeTWInitialSmartJobService realtimeTWInitialSmartJobService,
            IRealtimeTWInitialSmartBySportJobService realtimeTWInitialSmartBySportJobService)
        {
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
            this.normalDangerousAssociationCheckJobService = normalDangerousAssociationCheckJobService;
            this.normalDangerousAssociationScanJobService = normalDangerousAssociationScanJobService;

            this.robotAccountInsertJobService = robotAccountInsertJobService;
            this.tvsProblemAccountJobService = tvsProblemAccountJobService;
            this.mmClassificationJobService = mmClassificationJobService;
            this.detectUnauthorizedLoginCustJobService = detectUnauthorizedLoginCustJobService;
            this.dailyDangerousClassificationJobService = dailyDangerousClassificationJobService;
            this.realtimeTWInitialGroupBettingJobService = realtimeTWInitialGroupBettingJobService;
            this.realtimeTWInitialSmartJobService = realtimeTWInitialSmartJobService;
            this.realtimeTWInitialSmartBySportJobService = realtimeTWInitialSmartBySportJobService;
        }

        [HttpPost("scanningDailyNormalClassification")]
        public async Task<bool> DailyScanningNormalClassification()
        {
            LogExtension.LogInfo($"Daily Normal Service is running at {DateTime.Now}");
            return await this.dailyNormalClassificationJobService.Execute();
        }

        [HttpPost("scanningDailyProblemClassification")]
        public async Task<bool> DailyScanningProblemClassification([FromForm] int batchSize)
        {
            LogExtension.LogInfo($"Daily Problem Service is running at {DateTime.Now}");
            return await this.dailyProblemClassificationServices.Execute(batchSize);
        }

        [HttpPost("scanningDailyDangerousDepositClassification")]
        public async Task<bool> DailyScanningDangerousDepositClassification([FromForm] int batchSize)
        {
            LogExtension.LogInfo($"Daily Deposit Dangerous Score Classification Service is running at {DateTime.Now}");

            return await this.dailyDangerousClassificationJobService.Execute(batchSize, true);
        }

        [HttpPost("scanningDailyDangerousCreditClassification")]
        public async Task<bool> DailyScanningDangerousCreditClassification([FromForm] int batchSize)
        {
            LogExtension.LogInfo($"Daily Credit Dangerous Score Classification Service is running at {DateTime.Now}");

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
        public async Task<bool> NormalAccountInsert([FromForm] int batchExternalSize, [FromForm] int batchInternalSize, [FromForm] bool isLogData)
        {
            return await this.normalAccountJobService.Execute(batchExternalSize, batchInternalSize, isLogData);
        }

        [HttpPost("probationClassification")]
        public async Task<bool> ProbationScanningClassification([FromForm] int batchSize)
        {
            LogExtension.LogInfo($"Probation Service is running at {DateTime.Now}");
            return await this.probationClassificationJobService.Execute(batchSize);
        }

        [HttpPost("scanningDailyNormalTaggingClassification")]
        public async Task<bool> DailyScanningNormalTaggingClassification([FromForm] int batchSize)
        {
            LogExtension.LogInfo($"Daily Normal Tagging Service is running at {DateTime.Now}");
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

        [HttpPost("robotAIDetection")]
        public async Task<bool> RobotAIDetection([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.robotAccountInsertJobService.ScanningAIRobotClassification(batchExternalSize, batchInternalSize);
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

        [HttpPost("scanningMinutelyTWInitialSmartBySportClassification")]
        public async Task<bool> RealtimeTWInitialSmartBySportClassification([FromForm] int internalBatchSize, [FromForm] int externalBatchSize)
        {
            return await this.realtimeTWInitialSmartBySportJobService.Execute(internalBatchSize, externalBatchSize);
        }

        [HttpPost("normalDangerousAssociationCheck")]
        public async Task<bool> NormalDangerousAssociationCheck([FromForm] int batchSize)
        {
            return await this.normalDangerousAssociationCheckJobService.Execute(batchSize);
        }

        [HttpPost("normalDangerousAssociationScan")]
        public async Task<bool> NormalDangerousAssociationScan([FromForm] int internalBatchSize, [FromForm] int externalBatchSize)
        {
            return await this.normalDangerousAssociationScanJobService.Execute(internalBatchSize, externalBatchSize);
        }
    }
}