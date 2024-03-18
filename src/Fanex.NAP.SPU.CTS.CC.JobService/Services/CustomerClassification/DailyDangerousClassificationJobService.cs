namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDailyDangerousClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class DailyDangerousClassificationJobService : IDailyDangerousClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IDailyDangerousClassificationService dailyDangerousCCService;
        private readonly ISystemParameterServices systemParameterService;

        public DailyDangerousClassificationJobService(ISentryClient sentryClient,
            IDailyDangerousClassificationService dailyDangerousClassificationService,
            ISystemParameterServices systemParameterService)
        {
            this.sentryClient = sentryClient;
            this.dailyDangerousCCService = dailyDangerousClassificationService;
            this.systemParameterService = systemParameterService;
            this.serviceName = "DailyDangerousClassificationJobService";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                JobRun(batchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchSize)
        {
            var isLicensee = false;
            bool isContinue;

            do
            {
                var dangerousCCs = dailyDangerousCCService.GetDangerousClassification(isLicensee);

                isContinue = dangerousCCs.DangerousCCLastScanned != null && dangerousCCs.DangerousCCLastScanned.LastCustID > 0;

                if (isContinue)
                {
                    dailyDangerousCCService.InsertDangerousClassification(dangerousCCs.DangerousCCs, isLicensee, batchSize);

                    systemParameterService.UpdateTimingExecution(dangerousCCs.DangerousCCLastScanned.LastCustID.ToString(), Constants.CTS_SystemParameters.DailyDangerousClassification_LastCustID);

                    systemParameterService.UpdateTimingExecution(dangerousCCs.DangerousCCLastScanned.LastScannedDate?.ToString(Constants.CONVERT_TO_SQL_DATETIME), Constants.CTS_SystemParameters.DailyDangerousClassification_LastScannedDate);
                }                

            } while (isContinue);
        }

    }
}
