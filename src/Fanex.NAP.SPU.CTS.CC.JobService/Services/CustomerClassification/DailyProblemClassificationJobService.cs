namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    public interface IDailyProblemClassificationJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class DailyProblemClassificationJobService : IDailyProblemClassificationJobService
    {
        private readonly string serviceName;
        private readonly IDailyProblemClassificationServices dailyProblemClassificationServices;
        private readonly IAppSettings appSettings;

        public DailyProblemClassificationJobService(

            IDailyProblemClassificationServices dailyClassificationServices,
            IAppSettings appSettings)
        {
            this.dailyProblemClassificationServices = dailyClassificationServices;
            this.appSettings = appSettings;
            this.serviceName = "DailyProblemAccountProbationClassification";
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
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (batchSize), new Exception());
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
            var problemClassification = dailyProblemClassificationServices
                                            .Start()
                                            .GetPADailyScanFromQueue()
                                            .GetProblemAccountLosingClassification(FunctionId.ProblemDailyJob);

            if (problemClassification != null && problemClassification.Any())
            {
                var batchChunks = problemClassification.Chunk(batchSize);

                foreach (var problemAccountBatch in batchChunks)
                {
                    var isSuccess = dailyProblemClassificationServices
                                        .Reset()
                                        .InsertProblemAccountProbationClassification(batchSize, problemAccountBatch)
                                        .InsertPALogic()
                                        .PushCCToExternal(new PushDataToExternalEntity
                                        {
                                            RoleGroup = RoleGroupType.Member,
                                            ExternalBatchSize = batchSize,
                                            IsUpdateDangerToMainDB = true,
                                            IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                                            ServiceName = serviceName
                                        });

                    if (!isSuccess)
                    {
                        return;
                    }
                }
            }

            var isContinue = dailyProblemClassificationServices.UpdateComplete();

            if (isContinue)
            {
                JobRun(batchSize);
            }
        }
    }

    #endregion Private Methods
}