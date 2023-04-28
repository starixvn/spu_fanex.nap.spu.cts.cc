namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
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
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva;
    using Renovate = Features.CustomerClassification.Renovate;

    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterNAPService(services);

            RegisterProblemAccount(services);

            RegisterDataAccesLayer(services);

            RegisterNormalAccount(services);

            RegisterDailyNormalClassification(services);

            RegisterDailyProblemClassification(services);

            RegisterRealtimeClassification(services);

            RegisterProbationClassification(services);

            RegisterRobotAccountInsert(services);

            RegisterCustomer(services);

            RegisterTWGroupBetting(services);

            RegisterTWProblemAccount(services);

            RegisterAffectedCustomerClassification(services);

            RegisterNewCTSCustomerClassification(services);

            RegisterTVSProblemAccountInsert(services);

            RegisterClassifyGBMatchMonitor(services);

            RegisterDataLogTracking(services);

            RegisterSabaDangerLevel(services);

            RegisterNormalAccountBySport(services);

            RegisterDailyNormalClassificationBySport(services);

            RegisterProbationClassificationBySport(services);

            RegisterAssociatedGroupMonitor(services);

            RegisterClassificationJobMonitor(services);

            RegisterCustomerJobMonitor(services);

            RegisterRobotImperva(services);

            RegisterRealtimeClassificationRenovate(services);

            RegisterRealtimeCheckChangeClassificationRenovate(services);

            RegisterNormalAccountClassification(services);

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

        private static void RegisterNormalAccount(this IServiceCollection services)
        {
            services.AddSingleton<INormalAccountDataAccess, NormalAccountDataAccess>();
            services.AddSingleton<INormalAccountServices, NormalAccountServices>();
            services.AddSingleton<INormalAccountJobService, NormalAccountJobService>();
        }

        private static void RegisterDailyNormalClassification(this IServiceCollection services)
        {
            services.AddSingleton<IDailyNormalClassificationDataAccess, DailyNormalClassificationDataAccess>();
            services.AddSingleton<IDailyNormalClassificationServices, DailyNormalClassificationServices>();
            services.AddSingleton<IDailyNormalClassificationJobService, DailyNormalClassificationJobService>();
        }

        private static void RegisterDailyProblemClassification(this IServiceCollection services)
        {
            services.AddSingleton<IDailyProblemClassificationDataAccess, DailyProblemClassificationDataAccess>();
            services.AddSingleton<IDailyProblemClassificationServices, DailyProblemClassificationServices>();
            services.AddSingleton<IDailyProblemClassificationJobService, DailyProblemClassificationJobService>();
        }

        private static void RegisterRealtimeClassification(this IServiceCollection services)
        {
            services.AddSingleton<IRealtimeClassificationDataAccess, RealtimeClassificationDataAccess>();
            services.AddSingleton<IRealtimeClassificationServices, RealtimeClassificationServices>();
            services.AddSingleton<IRealtimeClassificationJobService, RealtimeClassificationJobService>();

            services.AddSingleton<IRealtimeClassificationBySportDataAccess, RealtimeClassificationBySportDataAccess>();
            services.AddSingleton<IRealtimeClassificationBySportServices, RealtimeClassificationBySportServices>();
            services.AddSingleton<IRealtimeClassificationBySportJobService, RealtimeClassificationBySportJobService>();
        }

        private static void RegisterProbationClassification(this IServiceCollection services)
        {
            services.AddSingleton<IProbationClassificationDataAccess, ProbationClassificationDataAccess>();
            services.AddSingleton<IProbationClassificationServices, ProbationClassificationServices>();
            services.AddSingleton<IProbationClassificationJobService, ProbationClassificationJobService>();
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

        private static void RegisterAffectedCustomerClassification(this IServiceCollection services)
        {
            services.AddSingleton<IAffectedDownlineCustomerClassificationDataAccess, AffectedDownlineCustomerClassificationDataAccess>();
            services.AddSingleton<IAffectedDownlineCustomerClassificationService, AffectedDownlineCustomerClassificationService>();
            services.AddSingleton<IAffectedDownlineCustomerClassificationJobService, AffectedDownlineCustomerClassificationJobService>();
        }

        private static void RegisterNewCTSCustomerClassification(this IServiceCollection services)
        {
            services.AddSingleton<INewCustomerClassificationDataAccess, NewCustomerClassificationDataAccess>();
            services.AddSingleton<INewCustomerClassificationServices, NewCustomerClassificationServices>();
            services.AddSingleton<INewCustomerClassificationJobService, NewCustomerClassificationJobService>();
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

        private static void RegisterNormalAccountBySport(this IServiceCollection services)
        {
            services.AddSingleton<INormalAccountBySportDataAccess, NormalAccountBySportDataAccess>();
            services.AddSingleton<INormalAccountBySportService, NormalAccountBySportService>();
            services.AddSingleton<INormalAccountBySportJobService, NormalAccountBySportJobService>();
        }

        private static void RegisterDailyNormalClassificationBySport(this IServiceCollection services)
        {
            services.AddSingleton<IDailyNormalClassificationBySportDataAccess, DailyNormalClassificationBySportDataAccess>();
            services.AddSingleton<IDailyNormalClassificationBySportServices, DailyNormalClassificationBySportServices>();
            services.AddSingleton<IDailyNormalClassificationBySportJobService, DailyNormalClassificationBySportJobService>();
        }

        private static void RegisterProbationClassificationBySport(this IServiceCollection services)
        {
            services.AddSingleton<IProbationClassificationBySportDataAccess, ProbationClassificationBySportDataAccess>();
            services.AddSingleton<IProbationClassificationBySportService, ProbationClassificationBySportService>();
            services.AddSingleton<IProbationClassificationBySportJobService, ProbationClassificationBySportJobService>();
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

        private static void RegisterRealtimeClassificationRenovate(this IServiceCollection services)
        {
            services.AddSingleton<CustomerClassification.Renovate.IRealtimeClassificationJobService, 
                CustomerClassification.Renovate.RealtimeClassificationJobService>();
            services.AddSingleton<Renovate.RealtimeClassification.IRealtimeClassificationService, 
                Renovate.RealtimeClassification.RealtimeClassificationService>();
            services.AddSingleton<Renovate.RealtimeClassification.IRealtimeClassificationDataAccess, 
                Renovate.RealtimeClassification.RealtimeClassificationDataAccess>();
        }

        private static void RegisterRealtimeCheckChangeClassificationRenovate(this IServiceCollection services)
        {
            services.AddSingleton<CustomerClassification.Renovate.IRealtimeCheckChangeClassificationJobService, 
                CustomerClassification.Renovate.RealtimeCheckChangeClassificationJobService>();
            services.AddSingleton<Renovate.RealtimeCheckChangeClassification.IRealtimeCheckChangeClassificationService,
                Renovate.RealtimeCheckChangeClassification.RealtimeCheckChangeClassificationService>();
            services.AddSingleton<Renovate.RealtimeCheckChangeClassification.IRealtimeCheckChangeClassificationDataAccess,
                Renovate.RealtimeCheckChangeClassification.RealtimeCheckChangeClassificationDataAccess>();
        }

        private static void RegisterNormalAccountClassification(this IServiceCollection services)
        {
            services.AddSingleton<CustomerClassification.Renovate.INormalAccountClassificationJobService,
                CustomerClassification.Renovate.NormalAccountClassificationJobService>();
            services.AddSingleton<Renovate.NormalAccountClassification.INormalAccountClassificationDataAccess,
                Renovate.NormalAccountClassification.NormalAccountClassificationDataAccess>();
            services.AddSingleton<Renovate.NormalAccountClassification.INormalAccountClassificationService,
                Renovate.NormalAccountClassification.NormalAccountClassificationService>();
        }
    }
}
