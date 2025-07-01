namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Threading.Tasks;

    public interface IProbationClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class ProbationClassificationJobService : IProbationClassificationJobService
    {
        private readonly string serviceName;
        private readonly IProbationClassificationServices probationClassificationServices;

        public ProbationClassificationJobService(

            IProbationClassificationServices probationClassificationServices)
        {
            this.probationClassificationServices = probationClassificationServices;
            this.serviceName = "ProbationClassification";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                if (batchSize > 0)
                {
                    JobRun(batchSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (batchSize), new NullReferenceException());
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        #region Private Methods

        private void JobRun(int batchSize)
        {
            bool isContinue;

            do
            {
                isContinue = probationClassificationServices
                                .Start()
                                .GetProbationCustForScanning(batchSize)
                                .ClassifyProbation()
                                .UpdateLastScanDate();
            } while (isContinue);
        }
    }

    #endregion Private Methods
}