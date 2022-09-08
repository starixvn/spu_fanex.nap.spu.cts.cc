namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INormalAccountBySportJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);

        Task<bool> UpdateWriteLogSetting(string serviceName, bool isWriteLog);
    }

    public class NormalAccountBySportJobService : INormalAccountBySportJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountBySportService normalAccountBySportService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAppSettings appSettings;

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    JobRun(batchExternalSize, batchInternalSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (batchExternalSize, batchInternalSize), new NullReferenceException(), sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> UpdateWriteLogSetting(string serviceName, bool isWriteLog)
        {
            try
            {
                bool result = appSettings.UpdateIsWriteLogSetting(serviceName, isWriteLog);

                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                sentryClient.CaptureException(e);
                return Task.FromResult(false);
            }
        }

        private void JobRun(int batchExternalSize, int batchInternalSize)
        {
            var newClassification = normalAccountBySportService.GetNewClassification();


        }
    }
}
