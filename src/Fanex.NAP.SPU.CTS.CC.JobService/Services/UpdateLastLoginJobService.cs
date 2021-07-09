using Fanex.NAP.SPU.CTS.Features.UpdateLastLogin;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.CTS.JobService.Services
{
    public interface IUpdateLastLoginJobService
    {
        Task<bool> UpdateLastLoginTime();
    }

    public class UpdateLastLoginJobService : IUpdateLastLoginJobService
    {
        private readonly IUpdateLastLoginService uptLastLoginService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public UpdateLastLoginJobService(ISentryClient sentryClient, IUpdateLastLoginService uptLastLoginService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.uptLastLoginService = uptLastLoginService;
            this.appSettings = appSettings;
        }


        public Task<bool> UpdateLastLoginTime()
        {
            try
            {
                uptLastLoginService.UpdateLastLoginTime(appSettings.UpdateLastLoginBatchSize);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return Task.FromResult(false);
            }
        }
    }
}
