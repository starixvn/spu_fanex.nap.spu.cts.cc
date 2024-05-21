namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyDangerousClassificationJobService
    {
        Task<bool> Execute(int batchSize, bool isLicensee);
    }

    public class DailyDangerousClassificationJobService : IDailyDangerousClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IDailyDangerousClassificationService dailyDangerousCCService;
        private readonly ISystemParameterServices systemParameterService;
        private readonly IAppSettings appSettings;

        public DailyDangerousClassificationJobService(ISentryClient sentryClient,
            IDailyDangerousClassificationService dailyDangerousClassificationService,
            ISystemParameterServices systemParameterService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.dailyDangerousCCService = dailyDangerousClassificationService;
            this.systemParameterService = systemParameterService;
            this.appSettings = appSettings;
            this.serviceName = "DailyDangerousClassificationJobService";
        }

        public Task<bool> Execute(int batchSize, bool isLicensee)
        {
            try
            {
                JobRun(batchSize, isLicensee);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchSize, bool isLicensee)
        {
            bool isContinue;

            var paras = new PushDataToExternalEntity
            {
                ExternalBatchSize = batchSize,
                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                IsPushCCToSoon88 = true,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                ServiceName = serviceName
            };

            do
            {
                var dangerousCCs = dailyDangerousCCService.GetDangerousClassification(isLicensee);

                isContinue = dangerousCCs.DangerousCCLastScanned != null && dangerousCCs.DangerousCCLastScanned.LastCustID > 0;

                if (isContinue)
                {
                    dailyDangerousCCService.InsertDangerousClassification(dangerousCCs.DangerousCCs, isLicensee, paras);

                    systemParameterService.UpdateTimingExecution(dangerousCCs.DangerousCCLastScanned.LastCustID.ToString(), isLicensee ? CTS_SystemParameters.DangerousScoreClassification_Deposit_LastCustID : CTS_SystemParameters.DangerousScoreClassification_Credit_LastCustID);

                    systemParameterService.UpdateTimingExecution(dangerousCCs.DangerousCCLastScanned.LastScannedDate?.ToString(CONVERT_TO_SQL_DATETIME), isLicensee ? CTS_SystemParameters.DangerousScoreClassification_Deposit_LastScannedDate : CTS_SystemParameters.DangerousScoreClassification_Credit_LastScannedDate);
                }
            } while (isContinue);
        }
    }
}