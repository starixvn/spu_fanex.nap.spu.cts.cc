namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerJobMonitorJobService
    {
        Task<bool> Process(int batchSize, int externalSize);
    }

    public class CustomerJobMonitorJobService : ICustomerJobMonitorJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly ICustomerJobMonitorService customerJobMonitorService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly IAppSettings appSettings;

        public CustomerJobMonitorJobService(
            ISentryClient sentryClient,
            ICustomerJobMonitorService customerJobMonitorService,
            ISystemParameterServices systemParameterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.customerJobMonitorService = customerJobMonitorService;
            this.systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
            this.serviceName = "CustomerJobMonitor";
        }

        public Task<bool> Process(int batchSize, int externalSize)
        {
            try
            {
                var lastScannedCustID = systemParameterServices.GetLastScannedId(SystemParameters.JobMonitor_NewCustomerLastScannedCustID);
                var lastScannedCustSubID = systemParameterServices.GetLastScannedId(SystemParameters.JobMonitor_NewCustomerLastScannedCustSubID);
                var fromDate = systemParameterServices.GetLastScannedDateTime(SystemParameters.JobMonitor_NewCustomerLastScannedTime);
                var toDate = fromDate?.AddMinutes(60);
                bool isSuccess = true;

                if (fromDate != null && toDate <= DateTime.Now.AddMinutes(-60))
                {
                    bool isContinue = true; 

                    while(isContinue && isSuccess)
                    {
                        var mainCusts = customerJobMonitorService.GetCustInfoFromMainDBForMonitor(externalSize, toDate.Value, lastScannedCustID);
                        var custSubs = customerJobMonitorService.GetCustSubInfoFromMainDBForMonitor(externalSize, toDate.Value, lastScannedCustSubID);
                        var customers = mainCusts.NewCustomers?.ToList().Concat(custSubs.NewCustomers);

                        if (customers != null && customers.Any())
                        {
                            lastScannedCustID = mainCusts.MaxScannedCustomerID ?? 0;
                            lastScannedCustSubID = custSubs.MaxScannedCustomerID ?? 0;

                            var missingCustomers = new List<MissingCustomerCompareEntity>();

                            var customerChunks = customers.Chunk(batchSize);

                            foreach (var custBatch in customerChunks)
                            {
                                missingCustomers.AddRange(customerJobMonitorService.CheckMissingCustomer(custBatch));
                            }

                            isSuccess = MonitoringNewCustomers(new InsertMissingCustomerModel
                            {
                                CustomerInfo = customers,
                                MissingCustomers = missingCustomers,
                                ScannedTimeFrom = fromDate.Value,
                                ScannedTimeTo = toDate.Value
                            }, batchSize);

                            UpdateLastScanCustomer(mainCusts.MaxScannedCustomerID, custSubs.MaxScannedCustomerID);
                        }
                        else
                        {
                            isContinue = false;
                        }
                    }    

                    if (isSuccess)
                    {
                        systemParameterServices.UpdateTimingExecution(toDate.Value.ToString(CONVERT_TO_SQL_DATETIME), SystemParameters.JobMonitor_NewCustomerLastScannedTime);
                    }
                }

                return Task.FromResult(isSuccess);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);

                return Task.FromResult(false);
            }
        }

        public bool MonitoringNewCustomers(InsertMissingCustomerModel newCustomers, int batchSize)
        {
            bool isSuccess = true;

            if (newCustomers.MissingCustomers.Any())
            {
                isSuccess = InsertMissingCustomer(newCustomers, batchSize);
            }

            if (isSuccess)
            {
                InsertNullCategoryCustomer(newCustomers, batchSize);
            }

            return isSuccess;
        }


        #region Private Methods

        private bool InsertMissingCustomer(InsertMissingCustomerModel newCustomers, int batchSize)
        {
            if (newCustomers.MissingCustomers.Any())
            {
                customerJobMonitorService.InsertMonitorMissingCust(newCustomers.MissingCustomers, newCustomers.ScannedTimeTo, batchSize);

                NotifyTelegramMessage(newCustomers.MissingCustomers.Select(c => c.CustID), JobMonitorType.NewCustomer_Missing, newCustomers.ScannedTimeFrom, newCustomers.ScannedTimeTo);
            }

            return true;
        }

        private void InsertNullCategoryCustomer(InsertMissingCustomerModel newCustomers, int batchSize)
        {
            if (newCustomers.CustomerInfo.Any())
            {
                var custToCheckCategory = newCustomers.CustomerInfo.Where(c => c.RoleID == 1 && !(newCustomers.MissingCustomers.Select(a => a.CustID).Contains(c.CustID)
                                                                                  && newCustomers.MissingCustomers.Select(a => a.CustSubID).Contains(c.CustSubID)));
                if (custToCheckCategory.Any())
                {
                    var nullCategory = GetNullCategoryCustomers(custToCheckCategory.Select(c => (long)c.CustID).Distinct(), batchSize);

                    customerJobMonitorService.InsertMonitorInvalidCustomerCategory(
                            nullCategory.Select(c => new IncorrectCustomerCategoriesMonitorEntity { CustID = c, CategoryID = null }),
                            newCustomers.ScannedTimeTo, JobMonitorType.NewCustomer_NullCategory, batchSize);

                    NotifyTelegramMessage(nullCategory, JobMonitorType.NewCustomer_NullCategory, newCustomers.ScannedTimeFrom, newCustomers.ScannedTimeTo);
                }
            }
        }

        private IEnumerable<long> GetNullCategoryCustomers(IEnumerable<long> custIds, int batchSize)
        {
            if (custIds != null & custIds.Any())
            {
                var custCategories = customerJobMonitorService.GetCustomerCategoriesToMonitor(custIds, batchSize);

                var nullCategories = custCategories.Where(c => c.CurrentCategoryID == null);

                return nullCategories.Any() ? nullCategories.Select(c => c.CustId) : Enumerable.Empty<long>();
            }

            return Enumerable.Empty<long>();
        }

        private void UpdateLastScanCustomer(long? maxScannedCustId, long? maxScannedCustSubId)
        {
            if (maxScannedCustId != null)
            {
                systemParameterServices.UpdateTimingExecution(maxScannedCustId.ToString(), SystemParameters.JobMonitor_NewCustomerLastScannedCustID);
            }

            if (maxScannedCustSubId != null)
            {
                systemParameterServices.UpdateTimingExecution(maxScannedCustSubId.ToString(), SystemParameters.JobMonitor_NewCustomerLastScannedCustSubID);
            }
        }

        private void NotifyTelegramMessage(IEnumerable<long> custIds, JobMonitorType type, DateTime scannedTimeFrom, DateTime scannedTimeTo)
        {
            if (custIds.Any())
            {
                var message = string.Empty;

                switch (type)
                {
                    case JobMonitorType.NewCustomer_NullCategory:
                        {
                            message = $"Please kindly check the following custid with category NULL - {{Total custid: {custIds.Distinct().Count()}}} - {string.Join(',', custIds.Distinct().Take(50))} - " +
                                      $"Scan from  \"{scannedTimeFrom:dd/MM/yyyy HH:mm:ss}\" to  \"{scannedTimeTo:dd/MM/yyyy HH:mm:ss}\"";
                            break;
                        }
                    case JobMonitorType.NewCustomer_Missing:
                        {
                            message = $"Please kindly check the following custid with not available on CTS - {{Total custid: {custIds.Distinct().Count()}}} - {string.Join(',', custIds.Distinct().Take(50))} - " +
                                      $"Scan from \"{scannedTimeFrom:dd/MM/yyyy HH:mm:ss}\" to \"{scannedTimeTo:dd/MM/yyyy HH:mm:ss}\"";
                            break;
                        }
                }

                Utilities.NotifyTelegramMessage(message, "Monitoring New Customer", appSettings.EvironmentName, appSettings.EvironmentName == "LOCAL" ? Telegram_GroupId_Local : Telegram_JobMonitor_GroupId_Pro);
            }
        }

        #endregion
    }
}