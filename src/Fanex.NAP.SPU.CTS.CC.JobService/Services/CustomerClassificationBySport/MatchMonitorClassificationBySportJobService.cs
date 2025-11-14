namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;    
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMatchMonitorClassificationBySportJobService
    {
        Task<bool> Execute();
    }

    public class MatchMonitorClassificationBySportJobService : IMatchMonitorClassificationBySportJobService
    {
        private readonly IMatchMonitorClassificationBySportService mmClassificationService;     
        private readonly IAppSettings appSettings;
        private readonly string serviceName;
        private const int batchSize = 500;

        public MatchMonitorClassificationBySportJobService(
            IAppSettings appSettings,
            IMatchMonitorClassificationBySportService mmClassificationService)
        {
            this.appSettings = appSettings;
            this.serviceName = "Match Monitor Classification By Sport";
            this.mmClassificationService = mmClassificationService;         
        }

        public Task<bool> Execute()
        {
            try
            {
                var classifyMMModel = mmClassificationService.ClassifyMatchMonitor();

                if (classifyMMModel.CustInfos != null && classifyMMModel.CustInfos.Any())
                {    
                    var problemAccounts = classifyMMModel.CustInfos.Select(x => new InsertProblemAccountBySportEntity(x));

                    mmClassificationService.InsertProblemAccount(problemAccounts,
                        new PushDataToExternalEntity
                        {
                            RoleGroup = RoleGroupType.Member,
                            ExternalBatchSize = batchSize,
                            IsUpdateDangerToMainDB = true,
                            IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                            ServiceName = serviceName
                        });                               
                }

                mmClassificationService.ClassifyCompletedMatchMonitor(classifyMMModel);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex);

                return Task.FromResult(false);
            }
        }
    }
}
