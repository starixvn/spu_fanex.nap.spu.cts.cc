namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;    
    using Sentry;
    using System;    
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerCategoryJobService
    {
        Task<bool> Execute();
    }

    public class CustomerCategoryJobService : ICustomerCategoryJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly ICustomerCategoryService customerCategoryService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly IAppSettings appSettings;

        public CustomerCategoryJobService(ISentryClient sentryClient
            , IAppSettings appSettings
            , ICustomerCategoryService customerCategoryService
            , ICustClassToMainDBService custClassToMainDBService
            , IAntiFraudCenterServices antiFraudCenterServices)
        {
            this.sentryClient = sentryClient;
            this.customerCategoryService = customerCategoryService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.appSettings = appSettings;
        }

        public Task<bool> Execute()
        {
            try
            {
                var custCategoryQueues = customerCategoryService.GetCustomerCategoryQueue();

                if (custCategoryQueues != null && custCategoryQueues.CustomerCategoryQueue.Any())
                {
                    var lastestCustomerClassification = customerCategoryService.AddCustomerCategory(custCategoryQueues);

                    Parallel.Invoke(() =>
                    {
                        var pushCCResult = custClassToMainDBService.RetryPushCCToMainDB(lastestCustomerClassification, 1, "Push CC To MainDB - IsApplied Lastest CC");
                    },
                    () =>
                    {
                        var custIds = lastestCustomerClassification.Select(x => x.CustId);
                        antiFraudCenterServices.DeleteAFCAccounts(custIds, FromFlow.CTSServices, appSettings.IsSyncAFCData, appSettings.AFCDeleteAccountURL);
                    });
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
