namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDailyProblemClassificationBySportJobService
    {
        Task<bool> Excute(int batchSize);
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

        public Task<bool> Excute(int batchSize)
        {
            try
            {
                if (batchSize > 0)
                {
                    JobRun(batchSize);
                }
                else
                {
                    LogExtension.LogError($"{_serviceName} Invalid Batch size {{{batchSize}}}");
                }
            }
            catch (Exception ex)
            {
                ex.LogError($"{_serviceName} Error");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchSize)
        {
            var isContinue = true;

            do
            {
                var paClassificationBySport =
                    _service
                        .Start()
                        .GetDailyPAFromQueue()
                        .GetProblemAccountLosingClassification();

                if (paClassificationBySport.IsNotNullOrEmpty())
                {
                    foreach (var paBatch in paClassificationBySport.Chunk(batchSize))
                    {
                        var isPushCC = _service
                            .GetInstanceProblemWLClassificationBySportService()
                            .InsertPAWLClassificationBySport(paBatch)
                            .PushCCBySportToExternal(_serviceName, batchSize);

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
