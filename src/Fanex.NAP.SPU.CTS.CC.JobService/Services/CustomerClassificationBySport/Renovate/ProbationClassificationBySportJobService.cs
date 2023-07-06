namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.ProbationClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IProbationClassificationBySportJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class ProbationClassificationBySportJobService : IProbationClassificationBySportJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IProbationClassificationBySportService probationClassificationServices;

        public ProbationClassificationBySportJobService(ISentryClient sentryClient
            , IProbationClassificationBySportService probationClassificationServices)
        {
            this.sentryClient = sentryClient;
            this.probationClassificationServices = probationClassificationServices;
            this.serviceName = "ProbationClassificationBySportRenovate";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                var isContinue = true;

                if (batchSize <= 0)
                    batchSize = 500;

                do
                {
                    isContinue = JobRun(batchSize);

                } while (isContinue);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool JobRun(int batchSize)
        {
            probationClassificationServices
                .Start()
                .GetProbationScanClassificationBySport(batchSize)
                .PreprocessProbationClassificationBySport();

            return (probationClassificationServices.ProbationClassification != null &&
               probationClassificationServices.ProbationClassification.Any());
        }

    }
}
