namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationBySportJobService
    {
        Task<bool> Execute();
    }

    public class DailyNormalClassificationBySportJobService : IDailyNormalClassificationBySportJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IDailyNormalClassificationBySportServices dailyClassificationBySportServices;

        public DailyNormalClassificationBySportJobService(ISentryClient sentryClient,
            IDailyNormalClassificationBySportServices dailyClassificationBySportServices)
        {
            this.sentryClient = sentryClient;
            this.dailyClassificationBySportServices = dailyClassificationBySportServices;
            this.serviceName = "DailyNormalClassificationBySport";
        }

        public Task<bool> Execute()
        {
            try
            {
                var isContinue = true;

                do
                {
                    isContinue = JobRun();

                } while (isContinue);                    
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool JobRun()
        {
            return this.dailyClassificationBySportServices.Start()
                                                       .GetCustClassificationForDailyScanning()
                                                       .ScanningDailyNormalClassification(FunctionId.NormalDailyJob)
                                                       .UpdateLastScan();            
        }
    }
}
