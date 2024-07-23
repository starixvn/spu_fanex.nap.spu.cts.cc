namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IProbationClassificationBySportJobService
    {
        Task<bool> Execute();
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
            this.serviceName = "ProbationClassificationBySport";
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
            probationClassificationServices
                .Start()
                .GetProbationScanClassificationBySport()
                .PreprocessProbationClassificationBySport()
                .UpdateCompleteProbationScanBySport();

            return (probationClassificationServices.ProbationClassification != null &&
               probationClassificationServices.ProbationClassification.Any());
        }

    }
}
