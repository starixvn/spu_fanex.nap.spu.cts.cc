namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
 	using Sentry;
    using System;
    using System.Threading.Tasks;
    using Features.TWClassification;

    public interface ITWClassificationJobService
    {
        Task<bool> ScanTWSpecialLicSubCC(int batchInternalSize);

        Task<bool> TWResponseHaifaService();
    }

    public class TWClassificationJobService : ITWClassificationJobService
    {        
        private readonly ISentryClient _sentryClient;
        private readonly IAppSettings _appSettings;
        private readonly ITWClassificationService _twClassificationService;

        public TWClassificationJobService(ISentryClient sentryClient,
            IAppSettings appSettings,
            ITWClassificationService twClassificationService)
        {            
            _sentryClient = sentryClient;
            _appSettings = appSettings;
            _twClassificationService = twClassificationService;         
        }
        public Task<bool> TWResponseHaifaService()
        {
            return _twClassificationService.SendDataToTW();
        }

        public Task<bool> ScanTWSpecialLicSubCC(int batchInternalSize)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
