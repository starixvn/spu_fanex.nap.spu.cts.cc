namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel;
    using Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva;
    //using Renovate = Features.CustomerClassification.Renovate;
    using RenovateBySport = Features.CustomerClassificationBySport.Renovate;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalTaggingClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline;

    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterNAPService(services);

            RegisterProblemAccount(services);

            RegisterDataAccesLayer(services);        

            RegisterDailyProblemClassification(services);       

            RegisterRobotAccountInsert(services);

            RegisterCustomer(services);

            RegisterTWGroupBetting(services);

            RegisterTWProblemAccount(services);

            RegisterTVSProblemAccountInsert(services);

            RegisterClassifyGBMatchMonitor(services);

            RegisterDataLogTracking(services);

            RegisterSabaDangerLevel(services);   

            RegisterAssociatedGroupMonitor(services);

            RegisterClassificationJobMonitor(services);

            RegisterCustomerJobMonitor(services);

            RegisterRobotImperva(services);

            RegisterRealtimeClassification(services);

            RegisterRealtimeCheckChangeClassification(services);

            RegisterNormalAccountClassification(services);

            RegisterNormalAccount(services);

            RegisterNormalTaggingClassification(services);

            RegisterRescanClassification(services);

            RegisterDailyNormal(services);

            RegisterProbation(services);

            RegisterAffectedDownline(services);

            RegisterNewCustomerClassification(services);

            RegisterNormalAccountClassificationBySportRenovate(services);

            RegisterNormalAccountInsertBySportRenovate(services);

            RegisterRealtimeClassificationBySportRenovate(services);

            RegisterProbationBySportRenovate(services);

            RegisterDailyNormalBySportRenovate(services);

            RegisterRescanBySportRenovate(services);

            RegisterNewTWGroupBettingClassificationRenovate(services);

            RegisterDetectUnauthorizedLoginCustomer(services);

            RegisterDailyDangerousClassification(services);
        }       

        private static void RegisterDetectUnauthorizedLoginCustomer(IServiceCollection services)
        {
            services.AddSingleton<IDetectUnauthorizedLoginCustomerDataAccess, DetectUnauthorizedLoginCustomerDataAccess>();            
            services.AddSingleton<IDetectUnauthorizedLoginCustomerService, DetectUnauthorizedLoginCustomerService>();
            services.AddSingleton<IDetectUnauthorizedLoginCustomerJobService, DetectUnauthorizedLoginCustomerJobService>();
        }

        private static void RegisterNewTWGroupBettingClassificationRenovate(IServiceCollection services)
        {
            services.AddSingleton<ITWGBTaggingClassificationDataAccess, TWGBTaggingClassificationDataAccess>();
            services.AddSingleton<ITWGBTaggingClassificationService, TWGBTaggingClassificationService>();
            services.AddSingleton<ITWGBTaggingClassificationJobService, TWGBTaggingClassificationJobService>();
        }

        private static void RegisterTWGroupBetting(this IServiceCollection services)
        {
            services.AddSingleton<ITWGroupBettingDataAccess, TWGroupBettingDataAccess>();
            services.AddSingleton<ITWGroupBettingService, TWGroupBettingService>();
            services.AddSingleton<ITWGroupBettingJobService, TWGroupBettingJobService>();
        }

        private static void RegisterTWProblemAccount(this IServiceCollection services)
        {
            services.AddSingleton<ITWProblemAccountDataAccess, TWProblemAccountDataAccess>();
            services.AddSingleton<ITWProblemAccountService, TWProblemAccountService>();
            services.AddSingleton<ITWProblemAccountJobService, TWProblemAccountJobService>();
        }

        private static void RegisterNAPService(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<INapSession, NapSession>();
            services.AddSingleton<ISecurityService, SecurityService>();
        }

        private static void RegisterProblemAccount(this IServiceCollection services)
        {
            services.AddSingleton<IProblemAccountDataAccess, ProblemAccountDataAccess>();
            services.AddSingleton<IProblemAccountServices, ProblemAccountServices>();
        }      

        private static void RegisterDailyProblemClassification(this IServiceCollection services)
        {
            services.AddSingleton<IDailyProblemClassificationDataAccess, DailyProblemClassificationDataAccess>();
            services.AddSingleton<IDailyProblemClassificationServices, DailyProblemClassificationServices>();
            services.AddSingleton<IDailyProblemClassificationJobService, DailyProblemClassificationJobService>();
        }     

        private static void RegisterRobotAccountInsert(this IServiceCollection services)
        {
            services.AddSingleton<IRobotTWInsertDataAccess, RobotTWInsertDataAccess>();
            services.AddSingleton<IRobotTWInsertServices, RobotTWInsertServices>();

            services.AddSingleton<IRobotAccountInsertJobService, RobotAccountInsertJobService>();

            services.AddSingleton<IRobotAIDetectionDataAccess, RobotAIDetectionDataAccess>();
            services.AddSingleton<IRobotAIDetectionService, RobotAIDetectionService>();
        }

        private static void RegisterCustomer(this IServiceCollection services)
        {
            services.AddSingleton<ICTSCustomerDataAccess, CTSCustomerDataAccess>();
            services.AddSingleton<ICTSCustomerService, CTSCustomerService>();
            services.AddSingleton<ICTSCustomerJobService, CTSCustomerJobService>();
        }

        private static void RegisterDataAccesLayer(this IServiceCollection services)
        {
            services.AddSingleton<ISystemParameterServices, SystemParameterServices>();
            services.AddSingleton<ISystemParameterDataAccess, SystemParameterDataAccess>();

            services.AddSingleton<ICustClassToMainDBDataAccess, CustClassToMainDBDataAccess>();
            services.AddSingleton<ICustClassToMainDBService, CustClassToMainDBService>();

            services.AddSingleton<IFeedSoon88DataAccess, FeedSoon88DataAccess>();
            services.AddSingleton<IFeedSoon88Services, FeedSoon88Services>();

            services.AddSingleton<IRescanCustClassificationDataAccess, RescanCustClassificationDataAccess>();
            services.AddSingleton<IRescanCustClassificationService, RescanCustClassificationService>();

            services.AddSingleton<IAntiFraudCenterServices, AntiFraudCenterServices>();
        }            

        private static void RegisterTVSProblemAccountInsert(this IServiceCollection services)
        {
            services.AddSingleton<ITVSProblemAccountDataAccess, TVSProblemAccountDataAccess>();
            services.AddSingleton<ITVSProblemAccountServices, TVSProblemAccountServices>();
            services.AddSingleton<ITVSProblemAccountJobService, TVSProblemAccountJobService>();
        }

        private static void RegisterClassifyGBMatchMonitor(this IServiceCollection services)
        {
            services.AddSingleton<IMatchMonitorClassificationDataAccess, MatchMonitorClassificationDataAccess>();
            services.AddSingleton<IMatchMonitorClassificationService, MatchMonitorClassificationService>();
            services.AddSingleton<IMatchMonitorClassificationJobService, MatchMonitorClassificationJobService>();
        }

        private static void RegisterDataLogTracking(this IServiceCollection services)
        {
            services.AddSingleton<IDataLogTrackingDataAccess, DataLogTrackingDataAccess>();
            services.AddSingleton<IDataLogTrackingServices, DataLogTrackingServices>();
        }

        private static void RegisterSabaDangerLevel(this IServiceCollection services)
        {
            services.AddSingleton<ISabaDangerLevelDataAccess, SabaDangerLevelDataAccess>();
            services.AddSingleton<ISabaDangerLevelServices, SabaDangerLevelServices>();
            services.AddSingleton<ISabaDangerLevelJobService, SabaDangerLevelJobService>();
        }

        private static void RegisterAssociatedGroupMonitor(this IServiceCollection services)
        {
            services.AddSingleton<IAssociatedGroupMonitorDataAccess, AssociatedGroupMonitorDataAccess>();
            services.AddSingleton<IAssociatedGroupMonitorService, AssociatedGroupMonitorService>();
            services.AddSingleton<IAssociatedGroupMonitorJobService, AssociatedGroupMonitorJobService>();
        }

        private static void RegisterClassificationJobMonitor(this IServiceCollection services)
        {
            services.AddSingleton<IClassificationJobMonitorJobService, ClassificationJobMonitorJobService>();
            services.AddSingleton<IClassificationJobMonitorDataAccess, ClassificationJobMonitorDataAccess>();
            services.AddSingleton<IClassificationJobMonitorService, ClassificationJobMonitorService>();

            services.AddSingleton<IIncorrectDataMonitoringDataAccess, IncorrectDataMonitoringDataAccess>();
            services.AddSingleton<IIncorrectDataMonitoringService, IncorrectDataMonitoringService>();
            services.AddSingleton<IIncorrectDataMonitoringJobService, IncorrectDataMonitoringJobService>();
        }

        private static void RegisterCustomerJobMonitor(this IServiceCollection services)
        {
            services.AddSingleton<ICustomerJobMonitorJobService, CustomerJobMonitorJobService>();
            services.AddSingleton<ICustomerJobMonitorService, CustomerJobMonitorService>();
            services.AddSingleton<ICustomerJobMonitorDataAccess, CustomerJobMonitorDataAccess>();
        }

        private static void RegisterRobotImperva(this IServiceCollection services)
        {
            services.AddSingleton<ITWRobotImpervaJobService, TWRobotImpervaJobService>();
            services.AddSingleton<ITWRobotImpervaService, TWRobotImpervaService>();
            services.AddSingleton<ITWRobotImpervaDataAccess, TWRobotImpervaDataAccess>();
        }

        private static void RegisterRealtimeClassification(this IServiceCollection services)
        {
            services.AddSingleton<IRealtimeClassificationJobService, RealtimeClassificationJobService>();
            services.AddSingleton<IRealtimeClassificationService, RealtimeClassificationService>();
            services.AddSingleton<IRealtimeClassificationDataAccess, RealtimeClassificationDataAccess>();
        }

        private static void RegisterRealtimeCheckChangeClassification(this IServiceCollection services)
        {
            services.AddSingleton<IRealtimeCheckChangeClassificationJobService, 
                RealtimeCheckChangeClassificationJobService>();
            services.AddSingleton<IRealtimeCheckChangeClassificationService, RealtimeCheckChangeClassificationService>();
            services.AddSingleton<IRealtimeCheckChangeClassificationDataAccess, RealtimeCheckChangeClassificationDataAccess>();
        }

        private static void RegisterNormalAccountClassification(this IServiceCollection services)
        {
            services.AddSingleton<INormalAccountClassificationJobService, NormalAccountClassificationJobService>();
            services.AddSingleton<INormalAccountClassificationDataAccess, NormalAccountClassificationDataAccess>();
            services.AddSingleton<INormalAccountClassificationService, NormalAccountClassificationService>();
        }

        private static void RegisterNormalAccount(this IServiceCollection services)
        {
            services.AddSingleton<INormalAccountJobService, NormalAccountJobService>();
            services.AddSingleton<INormalAccountDataAccess, NormalAccountDataAccess>();
            services.AddSingleton<INormalAccountService, NormalAccountService>();
        }

        private static void RegisterNormalTaggingClassification(this IServiceCollection services)
        {
            services.AddSingleton<INormalTaggingClassificationJobService, NormalTaggingClassificationJobService>();
            services.AddSingleton<INormalTaggingClassificationDataAccess, NormalTaggingClassificationDataAccess>();
            services.AddSingleton<INormalTaggingClassificationServices, NormalTaggingClassificationServices>();
        }

        private static void RegisterRescanClassification(this IServiceCollection services)
        {
            services.AddSingleton<IRescanCustClassificationDataAccess, RescanCustClassificationDataAccess>();
            services.AddSingleton<IRescanCustClassificationService, RescanCustClassificationService>();
        }

        private static void RegisterProbation(this IServiceCollection services)
        {
            services.AddSingleton<IProbationClassificationDataAccess, ProbationClassificationDataAccess>();
            services.AddSingleton<IProbationClassificationServices, ProbationClassificationServices>();
            services.AddSingleton<IProbationClassificationJobService, ProbationClassificationJobService>();
        }

        private static void RegisterDailyNormal(this IServiceCollection services)
        {
            services.AddSingleton<IDailyNormalClassificationDataAccess, DailyNormalClassificationDataAccess>();
            services.AddSingleton<IDailyNormalClassificationServices, DailyNormalClassificationServices>();
            services.AddSingleton<IDailyNormalClassificationJobService, DailyNormalClassificationJobService>();
        }

        private static void RegisterAffectedDownline(this IServiceCollection services)
        {
            services.AddSingleton<IAffectedDownlineCustomerClassificationDataAccess, AffectedDownlineCustomerClassificationDataAccess>();
            services.AddSingleton<IAffectedDownlineCustomerClassificationService, AffectedDownlineCustomerClassificationService>();
            services.AddSingleton<IAffectedDownlineCustomerClassificationJobService, AffectedDownlineCustomerClassificationJobService>();
        }  
        
        private static void RegisterNewCustomerClassification(this IServiceCollection services)
        {
            services.AddSingleton<INewCustomerClassificationDataAccess, NewCustomerClassificationDataAccess>();
            services.AddSingleton<INewCustomerClassificationServices, NewCustomerClassificationServices>();
            services.AddSingleton<INewCustomerClassificationJobService, NewCustomerClassificationJobService>();
        }

        private static void RegisterNormalAccountClassificationBySportRenovate(this IServiceCollection services)
        {
            services.AddSingleton<RenovateBySport.NormalAccountClassificationBySport.INormalAccountClassificationBySportDataAccess,
                RenovateBySport.NormalAccountClassificationBySport.NormalAccountClassificationBySportDataAccess>();
            services.AddSingleton<RenovateBySport.NormalAccountClassificationBySport.INormalAccountClassificationBySportService,
                RenovateBySport.NormalAccountClassificationBySport.NormalAccountClassificationBySportService>();
            services.AddSingleton<CustomerClassificationBySport.Renovate.INormalAccountClassificationBySportJobService,
                CustomerClassificationBySport.Renovate.NormalAccountClassificationBySportJobService>();
        }

        private static void RegisterNormalAccountInsertBySportRenovate(this IServiceCollection services)
        {
            services.AddSingleton<RenovateBySport.NormalAccountInsertBySport.INormalAccountInsertBySportDataAccess,
                RenovateBySport.NormalAccountInsertBySport.NormalAccountInsertBySportDataAccess>();
            services.AddSingleton<RenovateBySport.NormalAccountInsertBySport.INormalAccountInsertBySportService,
                RenovateBySport.NormalAccountInsertBySport.NormalAccountInsertBySportService>();
            services.AddSingleton<CustomerClassificationBySport.Renovate.INormalAccountInsertBySportJobService,
                CustomerClassificationBySport.Renovate.NormalAccountInsertBySportJobService>();
        }

        private static void RegisterRealtimeClassificationBySportRenovate(this IServiceCollection services)
        {
            services.AddSingleton<RenovateBySport.RealtimeClassificationBySport.IRealtimeClassificationBySportDataAccess,
                RenovateBySport.RealtimeClassificationBySport.RealtimeClassificationBySportDataAccess>();
            services.AddSingleton<RenovateBySport.RealtimeClassificationBySport.IRealtimeClassificationBySportServices,
                RenovateBySport.RealtimeClassificationBySport.RealtimeClassificationBySportServices>();
            services.AddSingleton<CustomerClassificationBySport.Renovate.IRealtimeClassificationBySportJobService,
                CustomerClassificationBySport.Renovate.RealtimeClassificationBySportJobService>();
        }

        private static void RegisterProbationBySportRenovate(this IServiceCollection services)
        {
            services.AddSingleton<RenovateBySport.ProbationClassificationBySport.IProbationClassificationBySportDataAccess,
                RenovateBySport.ProbationClassificationBySport.ProbationClassificationBySportDataAccess>();
            services.AddSingleton<RenovateBySport.ProbationClassificationBySport.IProbationClassificationBySportService,
                RenovateBySport.ProbationClassificationBySport.ProbationClassificationBySportService>();
            services.AddSingleton<CustomerClassificationBySport.Renovate.IProbationClassificationBySportJobService,
                CustomerClassificationBySport.Renovate.ProbationClassificationBySportJobService>();
        }

        private static void RegisterDailyNormalBySportRenovate(this IServiceCollection services)
        {
            services.AddSingleton<RenovateBySport.DailyNormalClassificationBySport.IDailyNormalClassificationBySportDataAccess,
                RenovateBySport.DailyNormalClassificationBySport.DailyNormalClassificationBySportDataAccess>();
            services.AddSingleton<RenovateBySport.DailyNormalClassificationBySport.IDailyNormalClassificationBySportServices,
                RenovateBySport.DailyNormalClassificationBySport.DailyNormalClassificationBySportServices>();
            services.AddSingleton<CustomerClassificationBySport.Renovate.IDailyNormalClassificationBySportJobService,
                CustomerClassificationBySport.Renovate.DailyNormalClassificationBySportJobService>();
        }

        private static void RegisterRescanBySportRenovate(this IServiceCollection services)
        {
            services.AddSingleton<RenovateBySport.RescanNormalClassificationBySport.IRescanNormalClassificationBySportDataAccess,
                RenovateBySport.RescanNormalClassificationBySport.RescanNormalClassificationBySportDataAccess>();
            services.AddSingleton<RenovateBySport.RescanNormalClassificationBySport.IRescanNormalClassificationBySportServices,
                RenovateBySport.RescanNormalClassificationBySport.RescanNormalClassificationBySportServices>();
        }

        private static void RegisterDailyDangerousClassification(IServiceCollection services)
        {
            services.AddSingleton<IDailyDangerousClassificationJobService, DailyDangerousClassificationJobService>();
            services.AddSingleton<IDailyDangerousClassificationService, DailyDangerousClassificationService>();
            services.AddSingleton<IDailyDangerousClassificationDataAccess, DailyDangerousClassificationDataAccess>();
        }
    }
}
