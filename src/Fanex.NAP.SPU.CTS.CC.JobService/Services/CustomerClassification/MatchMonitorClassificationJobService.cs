namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;    
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMatchMonitorClassificationJobService
    {
        Task<bool> UpdateStatusMatchMonitor();

        Task<bool> ClassifyMatchMonitor();
    }

    public class MatchMonitorClassificationJobService : IMatchMonitorClassificationJobService
    {
        private readonly IMatchMonitorClassificationService mmClassificationService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly string serviceName;
        private const int batchSize = 500;

        public MatchMonitorClassificationJobService(ISentryClient sentryClient
            , IAppSettings appSettings
            , IMatchMonitorClassificationService mmClassificationService
            , IProblemAccountServices problemAccountServices
            , ICustClassToMainDBService custClassToMainDBService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.serviceName = "MatchMonitorClassicationStatus";
            this.mmClassificationService = mmClassificationService;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public Task<bool> UpdateStatusMatchMonitor()
        {
            try
            {
                var matcheIDs = mmClassificationService.GetMatchMonitor();

                var matchInfos = mmClassificationService.GetMatchInfosByIDs(string.Join(",", matcheIDs));

                if (matchInfos != null && matchInfos.Any())
                {
                    var matchStatus = matchInfos.Select(x => new MatchStatusModel
                    {
                        MatchID = x.MatchID,
                        EventStatus = x.EventStatus
                    });

                    mmClassificationService.UpdateStatusMatchMonitor(matchStatus);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }
        }

        public Task<bool> ClassifyMatchMonitor()
        {
            try
            {
                var classifyMMModel = mmClassificationService.ClassifyMatchMonitor();

                if (classifyMMModel.CustInfos != null && classifyMMModel.CustInfos.Any())
                {    
                    var problemAccounts = classifyMMModel.CustInfos.Select(x => new InsertProblemAccountEntity(x));

                    var custIdsResult = problemAccountServices.InsertProblemAccountPackage(problemAccounts
                        , FunctionId.ClassifyGBMatchMonitor
                        , CCInputFlow.InsertPACategory
                        , false
                        , serviceName);

                    custClassToMainDBService.PushExternalProblemAcountPackage(custIdsResult.Distinct(), new PushDataToExternalEntity
                    {
                        ExternalBatchSize = batchSize,
                        IsUpdateDangerToMainDB = true,
                        IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                        ServiceName = serviceName
                    });                   
                }

                mmClassificationService.ClassifyMatchMonitorCompleted(classifyMMModel);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }
        }
    }
}
