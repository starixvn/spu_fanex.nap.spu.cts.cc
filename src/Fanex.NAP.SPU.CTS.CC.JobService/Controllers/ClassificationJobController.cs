namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using Renovate = Services.CustomerClassification.Renovate;

    [ApiController]
    [Route("api/classificationJobs")]
    public class ClassificationJobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly Services.INormalAccountJobService normalAccountInsertJobService;
        private readonly Services.IDailyNormalClassificationJobService dailyNormalClassificationJobService;
        private readonly IDailyProblemClassificationJobService dailyProblemClassificationServices;
        private readonly Services.IRealtimeClassificationJobService realtimeClassificationJobService;

        private readonly IRealtimeCheckChangeClassificationJobService realtimeCheckChangeRenovateJobService;
        private readonly Renovate.IRealtimeClassificationJobService realtimeRenovateJobService;
        private readonly Renovate.INormalAccountClassificationJobService normalAccountClassificationRenovateJobService;
        private readonly Renovate.INormalAccountJobService normalAccountRenovateJobService;
        private readonly Renovate.IProbationClassificationJobService probationClassificationRenovateJobService;
        private readonly Renovate.IDailyNormalClassificationJobService dailyNormalClassificationRenovateJobService;
        private readonly Renovate.INewCustomerClassificationJobService newCustomerClassificationRenovateJobService;
        private readonly Renovate.IAffectedDownlineCustomerClassificationJobService affectedDownlineClassificationRenovateJobService;
        private readonly Renovate.INormalTaggingClassificationJobService normalTaggingClassificationRenovateJobService;

        private readonly Services.IProbationClassificationJobService probationClassificationJobService;
        private readonly IRobotAccountInsertJobService robotAccountInsertJobService;
        private readonly ITWProblemAccountJobService twProblemAccountJobService;
        private readonly ITVSProblemAccountJobService tvsProblemAccountJobService;
        private readonly Services.INewCustomerClassificationJobService newCustomerClassificationJobService;
        private readonly Services.IAffectedDownlineCustomerClassificationJobService affectedDownlineCustomerClassificationJobService;
        private readonly IMatchMonitorClassificationJobService mmClassificationJobService;
        private readonly ITWRobotImpervaJobService robotImpervaJobService;

        public ClassificationJobController(ISentryClient sentryClient,
            Services.INormalAccountJobService normalAccountInsertJobService,
            Services.IDailyNormalClassificationJobService dailyNormalClassificationJobService,
            IDailyProblemClassificationJobService dailyProblemClassificationServices,
            Services.IRealtimeClassificationJobService realtimeClassificationJobService,
            IRealtimeCheckChangeClassificationJobService realtimeCheckChangeRenovateJobService,
            Renovate.IRealtimeClassificationJobService realtimeRenovateJobService,
            Renovate.INormalAccountClassificationJobService normalAccountClassificationRenovateJobService,
            Renovate.INormalAccountJobService normalAccountRenovateJobService,
            Renovate.IProbationClassificationJobService probationClassificationRenovateJobService,
            Renovate.IDailyNormalClassificationJobService dailyNormalClassificationRenovateJobService,
            Renovate.INewCustomerClassificationJobService newCustomerClassificationRenovateJobService,
            Renovate.IAffectedDownlineCustomerClassificationJobService affectedDownlineClassificationRenovateJobService,
            Renovate.INormalTaggingClassificationJobService normalTaggingClassificationRenovateJobService,

            Services.IProbationClassificationJobService probationClassificationJobService,
            IRobotAccountInsertJobService robotAccountInsertJobService,
            ITWProblemAccountJobService twProblemAccountJobService,
            ITVSProblemAccountJobService tvsProblemAccountJobService,
            Services.INewCustomerClassificationJobService newCTSCustomerClassificationJobService,
            Services.IAffectedDownlineCustomerClassificationJobService affectedDownlineCustomerClassificationJobService,
            IMatchMonitorClassificationJobService mmClassificationJobService,
            ITWRobotImpervaJobService robotImpervaJobService)
        {
            this.sentryClient = sentryClient;
            this.normalAccountInsertJobService = normalAccountInsertJobService;
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;
            this.dailyNormalClassificationJobService = dailyNormalClassificationJobService;
            this.realtimeClassificationJobService = realtimeClassificationJobService;

            this.realtimeCheckChangeRenovateJobService = realtimeCheckChangeRenovateJobService;
            this.realtimeRenovateJobService = realtimeRenovateJobService;
            this.normalAccountClassificationRenovateJobService = normalAccountClassificationRenovateJobService;
            this.normalAccountRenovateJobService = normalAccountRenovateJobService;
            this.probationClassificationRenovateJobService = probationClassificationRenovateJobService;
            this.dailyNormalClassificationRenovateJobService = dailyNormalClassificationRenovateJobService;
            this.newCustomerClassificationRenovateJobService = newCustomerClassificationRenovateJobService;
            this.affectedDownlineClassificationRenovateJobService = affectedDownlineClassificationRenovateJobService;
            this.normalTaggingClassificationRenovateJobService = normalTaggingClassificationRenovateJobService;

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

        #region Renovate
        [HttpPost("realtimeCheckChangesRenovate")]
        public async Task<bool> RealtimeCheckChangesRenovate([FromForm] int batchSize)
        {
            return await this.realtimeCheckChangeRenovateJobService.Execute(batchSize);           
        }

        [HttpPost("scanningRealtimeClassificationRenovate")]
        public async Task<bool> RealtimeScanningClassificationRenovate([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.realtimeRenovateJobService.Execute(batchExternalSize, batchInternalSize);            
        }

        [HttpPost("normalAccountClassificationRenovate")]
        public async Task<bool> NormalAccountClassificationRenovate([FromForm] int batchInternalSize)
        {
            return await this.normalAccountClassificationRenovateJobService.Execute(batchInternalSize);            
        }

        [HttpPost("normalAccountInsertRenovate")]
        public async Task<bool> NormalAccountInsertRenovate([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            return await this.normalAccountRenovateJobService.Execute(batchExternalSize, batchInternalSize);            
        }

        [HttpPost("probationClassificationRenovate")]
        public async Task<bool> ProbationScanningClassificationRenovate([FromForm] int batchSize)
        {
            sentryClient.CaptureMessage($"Probation Service is running at {DateTime.Now}");
            return await this.probationClassificationRenovateJobService.Execute(batchSize);
        }

        [HttpPost("scanningDailyNormalClassificationRenovate")]
        public async Task<bool> DailyScanningNormalClassificationRenovate()
        {
            sentryClient.CaptureMessage($"Daily Normal Service is running at {DateTime.Now}");
            return await this.dailyNormalClassificationRenovateJobService.Execute();
        }

        [HttpPost("scanningDailyNormalTaggingClassificationRenovate")]
        public async Task<bool> DailyScanningNormalTaggingClassificationRenovate([FromForm] int batchExternalSize, [FromForm] int batchInternalSize)
        {
            sentryClient.CaptureMessage($"Daily Normal Tagging Service is running at {DateTime.Now}");
            return await this.normalTaggingClassificationRenovateJobService.Execute(batchExternalSize, batchInternalSize);
        }

        [HttpPost("newcustomerclassificationRenovate")]
        public async Task<bool> NewCustomerClassificationRenovate([FromForm] int batchSize)
        {
            return await newCustomerClassificationRenovateJobService.Execute(batchSize);
        }

        [HttpPost("affectedDownlineClassificationFromQueueRenovate")]
        public async Task<bool> AffectedDownlineClassificationFromQueueRenovate([FromForm] int batchSize)
        {
            return await this.affectedDownlineClassificationRenovateJobService.Execute(batchSize);
        }

        #endregion Renovate

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

        [HttpPost("robotImpervaGet")]
        public async Task<bool> RobotImpervaGet([FromForm] int batchSize)
        {           
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