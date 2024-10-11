namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRealtimeTWInitialSmartJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class RealtimeTWInitialSmartJobService : IRealtimeTWInitialSmartJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly ITWInitialSmartService tWInitialSmartService;
        private readonly ISystemParameterServices systemParameterServices;

        public RealtimeTWInitialSmartJobService(ISentryClient sentryClient
            , IAppSettings appSettings
            , ITWInitialSmartService tWInitialSmartService
            , ISystemParameterServices systemParameterServices)
        {
            this.sentryClient = sentryClient;
            this.tWInitialSmartService = tWInitialSmartService;
            this.appSettings = appSettings;
            this.systemParameterServices = systemParameterServices;
            this.serviceName = "Realtime TW Initial Smart JobService";
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
            bool isContinue;

            var paras = new PushDataToExternalEntity
            {
                RoleGroup = RoleGroupType.Member,
                ExternalBatchSize = batchSize,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                ServiceName = serviceName
            };

            do
            {
                var twInitialSmartCusts = tWInitialSmartService.GetCustInitialSmart();

                isContinue = twInitialSmartCusts.CustInfos != null && twInitialSmartCusts.CustInfos.Any();

                if (isContinue)
                {
                    tWInitialSmartService.InsertTWInitialSmartClassification(twInitialSmartCusts.CustInfos, paras);

                    systemParameterServices.UpdateTimingExecution(twInitialSmartCusts.NextScannedCustID.ToString(), CTS_SystemParameters.TWInitialSmartClassification_LastCustID);
                    systemParameterServices.UpdateTimingExecution(twInitialSmartCusts.NextScannedDateTime.ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.TWInitialSmartClassification_LastScanDate);
                }
            } while (isContinue);
        }
    }
}
