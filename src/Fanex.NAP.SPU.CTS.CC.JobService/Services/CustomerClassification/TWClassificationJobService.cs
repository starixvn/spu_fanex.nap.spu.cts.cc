namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
 	using Sentry;
    using System;
    using System.Threading.Tasks;
    using Features.TWClassification;
    using System.Linq;
    using Fanex.NAP.SPU.CTS.Shares;

    public interface ITWClassificationJobService
    {
        Task<bool> ScanTWSpecialLicSubCC(int batchInternalSize);

        Task<bool> TWResponseHaifaService();
    }

    public class TWClassificationJobService : ITWClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient _sentryClient;        
        private readonly ITWClassificationService _twClassificationService;

        public TWClassificationJobService(ISentryClient sentryClient,          
            ITWClassificationService twClassificationService)
        {            
            _sentryClient = sentryClient;            
            _twClassificationService = twClassificationService;
            serviceName = "TWClassification";
        }

        public Task<bool> TWResponseHaifaService()
        {
            return _twClassificationService.SendDataToTW();
        }

        public Task<bool> ScanTWSpecialLicSubCC(int batchInternalSize)
        {
            try
            {
                bool isContinue;

                do
                {
                    var custLicSubs = _twClassificationService.GetSpecialLicSubCCScan(batchInternalSize);

                    isContinue = custLicSubs != null && custLicSubs.Any();

                    if (isContinue)
                    {
                        _twClassificationService.PreprocessSpecialLicSubCCScan(custLicSubs);

                        _twClassificationService.CompleteSpecialLicSubCCScan(custLicSubs);
                    }
                } while (isContinue);                
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
