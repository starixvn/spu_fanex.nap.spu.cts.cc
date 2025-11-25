namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyProblemClassificationBySportJobService
    {
        Task<bool> Execute(int batchInternalSize, int batchExternalSize);
    }

    public class DailyProblemClassificationBySportJobService : IDailyProblemClassificationBySportJobService
    {
        private readonly string _serviceName;
        private readonly IDailyProblemClassificationBySportService _service;

        public DailyProblemClassificationBySportJobService(
            IDailyProblemClassificationBySportService service)
        {
            _service = service;

            _serviceName = "DailyProblemClassificationBySport";
        }

        public Task<bool> Execute(int batchInternalSize, int batchExternalSize)
        {
            try
            {
                if (batchInternalSize > 0 && batchExternalSize > 0)
                {
                    JobRun(batchInternalSize, batchExternalSize);
                }
                else
                {
                    LogExtension.LogError($"{_serviceName} Invalid Batch size Internal: {{{batchInternalSize}}}, External: {{{batchExternalSize}}}");
                }
            }
            catch (Exception ex)
            {
                ex.LogError($"{_serviceName} Error");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchInternalSize, int batchExternalSize)
        {
            var isContinue = true;

            do
            {
                var paClassificationBySport =
                    _service
                        .Start()
                        .GetDailyPAFromQueue(batchInternalSize)
                        .GetProblemAccountLosingClassification(BySport_FunctionId.ProblemDailyJob);

                if (paClassificationBySport.IsNotNullOrEmpty())
                {
                    foreach (var paBatch in paClassificationBySport.Chunk(batchExternalSize))
                    {
                        var isPushCC = _service
                            .GetInstanceProblemWLClassificationBySportService()
                            .InsertPAWLClassificationBySport(paBatch)
                            .PushCCBySportToExternal(_serviceName, batchExternalSize);

                        if (!isPushCC)
                        {
                            return;
                        }
                    }
                }

                isContinue = _service.UpdateComplete();
            }
            while (isContinue);
        }
    }
}
