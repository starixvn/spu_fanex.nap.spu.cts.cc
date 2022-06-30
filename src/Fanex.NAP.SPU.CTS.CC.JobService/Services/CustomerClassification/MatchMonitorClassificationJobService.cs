namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IMatchMonitorClassificationJobService
    {
        Task<bool> ClassifyGBMatchMonitor();
    }

    public class MatchMonitorClassificationJobService : IMatchMonitorClassificationJobService
    {
        private readonly IMatchMonitorClassificationService mmClassificationService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly string serviceName;

        public MatchMonitorClassificationJobService(ISentryClient sentryClient
            , IAppSettings appSettings
            , IMatchMonitorClassificationService mmClassificationService
            , IProblemAccountServices problemAccountServices)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.serviceName = "MatchMonitorClassicationStatus";
            this.mmClassificationService = mmClassificationService;
            this.problemAccountServices = problemAccountServices;
        }

        public Task<bool> ClassifyGBMatchMonitor()
        {
            try
            {
                var classifyGBMMModel = mmClassificationService.ClassifyGBMatchMonitor();

                if (classifyGBMMModel.CustInfos != null && classifyGBMMModel.CustInfos.Any())
                {    
                    var problemAccounts = classifyGBMMModel.CustInfos.Select(x => new InsertProblemAccountEntity(x));

                    problemAccountServices.InsertProblemAccountPackage(problemAccounts
                        , Shares.Constants.FunctionId.ClassifyGBMatchMonitor
                        , Shares.Constants.InsertPA_ByType.Category
                        , false
                        , serviceName);
                }

                mmClassificationService.ClassifyGBMatchMonitorCompleted(classifyGBMMModel);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }
        }
    }
}
