namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification;
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

            RegisteRobotAccountInsert(services);

            RegisteCustomer(services);

            RegisterTWGroupBetting(services);

            RegisterTWProblemAccount(services);

            RegisterCustomerCategory(services);

            RegisterAffectedCustomerClassification(services);

            RegisterNewCTSCustomerClassification(services);
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
            services.AddSingleton<IProblemAccountJobService, ProblemAccountJobService>();
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
        }

        private static void RegisterProbationClassification(this IServiceCollection services)
        {
            services.AddSingleton<IProbationClassificationDataAccess, ProbationClassificationDataAccess>();
            services.AddSingleton<IProbationClassificationServices, ProbationClassificationServices>();
            services.AddSingleton<IProbationClassificationJobService, ProbationClassificationJobService>();
        }

        private static void RegisteRobotAccountInsert(this IServiceCollection services)
        {
            services.AddSingleton<IRobotAccountInsertDataAccess, RobotAccountInsertDataAccess>();
            services.AddSingleton<IRobotAccountInsertServices, RobotAccountInsertServices>();

            services.AddSingleton<IRobotAIClassificationInsertDataAccess, RobotAIClassificationInsertDataAccess>();
            services.AddSingleton<IRobotAIClassificationInsertServices, RobotAIClassificationInsertServices>();
            services.AddSingleton<IRobotAccountInsertJobService, RobotAccountInsertJobService>();
        }

        private static void RegisteCustomer(this IServiceCollection services)
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

        private static void RegisterCustomerCategory(this IServiceCollection services)
        {
            services.AddSingleton<ICustomerCategoryJobService, CustomerCategoryJobService>();
            services.AddSingleton<ICustomerCategoryService, CustomerCategoryService>();
            services.AddSingleton<ICustomerCategoryDataAccess, CustomerCategoryDataAccess>();
        }

        private static void RegisterAffectedCustomerClassification(this IServiceCollection services)
        {
            services.AddSingleton<IAffectedCustomerClassificationDataAccess, AffectedCustomerClassificationDataAccess>();
            services.AddSingleton<IAffectedCustomerClassificationService, AffectedCustomerClassificationService>();
        }

        private static void RegisterNewCTSCustomerClassification(this IServiceCollection services)
        {
            services.AddSingleton<INewCTSClassificationDataAccess, NewCTSClassificationDataAccess>();
            services.AddSingleton<INewCTSClassificationServices, NewCTSClassificationServices>();
            services.AddSingleton<INewCTSCustomerClassificationJobService, NewCTSCustomerClassificationJobService>();
        }
    }
}