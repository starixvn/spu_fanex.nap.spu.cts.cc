namespace Fanex.NAP.SPU.CTS.JobService.Services
{
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.SPU.CTS.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.DailyClassification;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProbationClassification;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert;
    using Fanex.NAP.SPU.CTS.Features.Evidence.MultipleAccountRemoveEvidence;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.JobService.Services.Transform;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterNAPService(services);

            RegisterProblemAccount(services);

            RegisterDataAccesLayer(services);

            RegisterNormalAccount(services);

            RegisterDailyClassification(services);

            RegisterRealtimeClassification(services);

            RegisterProbationClassification(services);

            RegisteRobotAccountInsert(services);

            RegisteCustomerEvidencesInsert(services);

            RegisteRemoveAccountEvidences(services);

            RegisteCustomer(services);
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

        private static void RegisterDailyClassification(this IServiceCollection services)
        {
            services.AddSingleton<IDailyClassificationDataAccess, DailyClassificationDataAccess>();
            services.AddSingleton<IDailyClassificationServices, DailyClassificationServices>();
            services.AddSingleton<IDailyClassificationJobService, DailyClassificationJobService>();
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
            services.AddSingleton<IRobotAccountInsertJobService, RobotAccountInsertJobService>();
        }

        private static void RegisteRemoveAccountEvidences(this IServiceCollection services)
        {
            services.AddSingleton<IRemoveAccountEvidenceDataAccess, RemoveAccountEvidenceDataAccess>();
            services.AddSingleton<IRemoveAccountEvidenceServices, RemoveAccountEvidenceServices>();
            services.AddSingleton<IRemoveAccountEvidenceJobService, RemoveAccountEvidenceJobService>();
        }

        private static void RegisteCustomerEvidencesInsert(this IServiceCollection services)
        {
            services.AddSingleton<ICustomerEvidenceInsertDataAccess, CustomerEvidenceInsertDataAccess>();
            services.AddSingleton<ICustomerEvidenceInsertServices, CustomerEvidenceInsertServices>();
            services.AddSingleton<ICustomerEvidenceSendMailServices, CustomerEvidenceSendMailServices>();
            services.AddSingleton<ICustomerEvidenceInsertJobService, CustomerEvidenceInsertJobService>();
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
        }
    }
}