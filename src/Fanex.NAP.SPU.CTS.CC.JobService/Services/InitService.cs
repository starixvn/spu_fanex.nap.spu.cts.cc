namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalTaggingClassification;   
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RescanNormalClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
    using Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

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

            RegisterTVSProblemAccountInsert(services);

            RegisterClassifyGBMatchMonitor(services);

            RegisterDataLogTracking(services);

            RegisterSabaDangerLevel(services);   

            RegisterAssociatedGroupMonitor(services);

            RegisterClassificationJobMonitor(services);

            RegisterCustomerJobMonitor(services);

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

            RegisterNormalAccountClassificationBySport(services);

            RegisterNormalAccountInsertBySport(services);

            RegisterRealtimeClassificationBySport(services);

            RegisterProbationBySport(services);

            RegisterDailyNormalBySport(services);

            RegisterRescanBySport(services);        

            RegisterDetectUnauthorizedLoginCustomer(services);

            RegisterDailyDangerousClassification(services);

            RegisterTWInitialGroupBetting(services);

            RegisterTWInitialSmartClassification(services);

            RegisterWrapperCustomerClassificaton(services);
        }

        private static void RegisterWrapperCustomerClassificaton(IServiceCollection services)
        {
            services.AddSingleton<IBaseCustomerClassificationDataAccess, BaseCustomerClassificationDataAccess>();
            services.AddSingleton<IBaseCustomerClassificationService, BaseCustomerClassificationService>();

            services.AddSingleton<IBaseCustomerClassificationBySportDataAccess, BaseCustomerClassificationBySportDataAccess>();
            services.AddSingleton<IBaseCustomerClassificationBySportService, BaseCustomerClassificationBySportService>();
        }

        private static void RegisterTWInitialSmartClassification(IServiceCollection services)
        {
            services.AddSingleton<ITWInitialSmartDataAccess, TWInitialSmartDataAccess>();
            services.AddSingleton<ITWInitialSmartService, TWInitialSmartService>();
            services.AddSingleton<IRealtimeTWInitialSmartJobService, RealtimeTWInitialSmartJobService>();
        }
      
        private static void RegisterDetectUnauthorizedLoginCustomer(IServiceCollection services)
        {
            services.AddSingleton<IDetectUnauthorizedLoginCustomerDataAccess, DetectUnauthorizedLoginCustomerDataAccess>();            
            services.AddSingleton<IDetectUnauthorizedLoginCustomerService, DetectUnauthorizedLoginCustomerService>();
            services.AddSingleton<IDetectUnauthorizedLoginCustomerJobService, DetectUnauthorizedLoginCustomerJobService>();
        }
        
        private static void RegisterTWGroupBetting(this IServiceCollection services)
        {
            services.AddSingleton<ITWGroupBettingDataAccess, TWGroupBettingDataAccess>();
            services.AddSingleton<ITWGroupBettingService, TWGroupBettingService>();
            services.AddSingleton<ITWGroupBettingJobService, TWGroupBettingJobService>();
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
            services.AddSingleton<ICustomerClassificationAgencyService, CustomerClassificationAgencyService>();
            services.AddSingleton<IAffectedDownlineCustomerClassificationService, AffectedDownlineCustomerClassificationService>();
            services.AddSingleton<IAffectedDownlineCustomerClassificationJobService, AffectedDownlineCustomerClassificationJobService>();
            services.AddSingleton<ICustomerClassificationAgencyService, CustomerClassificationAgencyService>();
        }

        private static void RegisterNewCustomerClassification(this IServiceCollection services)
        {
            services.AddSingleton<INewCustomerClassificationDataAccess, NewCustomerClassificationDataAccess>();
            services.AddSingleton<INewCustomerClassificationServices, NewCustomerClassificationServices>();
            services.AddSingleton<INewCustomerClassificationJobService, NewCustomerClassificationJobService>();
        }

        private static void RegisterNormalAccountClassificationBySport(this IServiceCollection services)
        {
            services.AddSingleton<INormalAccountClassificationBySportDataAccess, NormalAccountClassificationBySportDataAccess>();
            services.AddSingleton<INormalAccountClassificationBySportService, NormalAccountClassificationBySportService>();
            services.AddSingleton<INormalAccountClassificationBySportJobService, NormalAccountClassificationBySportJobService>();
        }

        private static void RegisterNormalAccountInsertBySport(this IServiceCollection services)
        {
            services.AddSingleton<INormalAccountInsertBySportDataAccess, NormalAccountInsertBySportDataAccess>();
            services.AddSingleton<INormalAccountInsertBySportService, NormalAccountInsertBySportService>();
            services.AddSingleton<INormalAccountInsertBySportJobService, NormalAccountInsertBySportJobService>();
        }

        private static void RegisterRealtimeClassificationBySport(this IServiceCollection services)
        {
            services.AddSingleton<IRealtimeClassificationBySportDataAccess, RealtimeClassificationBySportDataAccess>();
            services.AddSingleton<IRealtimeClassificationBySportServices, RealtimeClassificationBySportServices>();
            services.AddSingleton<IRealtimeClassificationBySportJobService, RealtimeClassificationBySportJobService>();
        }

        private static void RegisterProbationBySport(this IServiceCollection services)
        {
            services.AddSingleton<IProbationClassificationBySportDataAccess, ProbationClassificationBySportDataAccess>();
            services.AddSingleton<IProbationClassificationBySportService, ProbationClassificationBySportService>();
            services.AddSingleton<IProbationClassificationBySportJobService, ProbationClassificationBySportJobService>();
        }

        private static void RegisterDailyNormalBySport(this IServiceCollection services)
        {
            services.AddSingleton<IDailyNormalClassificationBySportDataAccess, DailyNormalClassificationBySportDataAccess>();
            services.AddSingleton<IDailyNormalClassificationBySportServices, DailyNormalClassificationBySportServices>();
            services.AddSingleton<IDailyNormalClassificationBySportJobService, DailyNormalClassificationBySportJobService>();
        }

        private static void RegisterRescanBySport(this IServiceCollection services)
        {
            services.AddSingleton<IRescanNormalClassificationBySportDataAccess,
                RescanNormalClassificationBySportDataAccess>();
            services.AddSingleton<IRescanNormalClassificationBySportServices,
                RescanNormalClassificationBySportServices>();
        }

        private static void RegisterDailyDangerousClassification(IServiceCollection services)
        {
            services.AddSingleton<IDailyDangerousClassificationJobService, DailyDangerousClassificationJobService>();
            services.AddSingleton<IDailyDangerousClassificationService, DailyDangerousClassificationService>();
            services.AddSingleton<IDailyDangerousClassificationDataAccess, DailyDangerousClassificationDataAccess>();
        }

        private static void RegisterTWInitialGroupBetting(IServiceCollection services)
        {
            services.AddSingleton<IRealtimeTWInitialGroupBettingJobService, RealtimeTWInitialGroupBettingJobService>();
            services.AddSingleton<ITWInitialGroupBettingDataAccess, TWInitialGroupBettingDataAccess>();
            services.AddSingleton<ITWInitialGroupBettingService, TWInitialGroupBettingService>();
        }
    }
}
