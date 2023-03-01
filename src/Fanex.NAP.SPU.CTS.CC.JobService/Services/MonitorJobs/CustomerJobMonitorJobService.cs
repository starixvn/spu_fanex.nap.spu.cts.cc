namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerJobMonitorJobService
    {
        Task<bool> MonitorNewCustomerJob(int batchSize);
    }

    public class CustomerJobMonitorJobService : ICustomerJobMonitorJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly ICustomerJobMonitorService customerJobMonitorService;
        private readonly IClassificationJobMonitorService classificationJobMonitorService;
        private readonly ICTSCustomerService customerService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly IAppSettings appSettings;

        public CustomerJobMonitorJobService(
            ISentryClient sentryClient,
            ICustomerJobMonitorService customerJobMonitorService,
            IClassificationJobMonitorService classificationJobMonitorService,
            ICTSCustomerService customerService,
            ISystemParameterServices systemParameterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.customerJobMonitorService = customerJobMonitorService;
            this.classificationJobMonitorService = classificationJobMonitorService;
            this.customerService = customerService;
            this.systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
            this.serviceName = "CustomerJobMonitor";
        }

        public Task<bool> MonitorNewCustomerJob(int batchSize)
        {
            try
            {
                var isAutoInsertMissingCustomer = appSettings.IsAutoInsertMissingCustomer;
                var fromDate = systemParameterServices.GetLastScannedDateTime(SystemParameters.JobMonitor_InsertNewCustomerLastScannedTime);
                var toDate = fromDate?.AddMinutes(60);
                bool isSuccess = true;

                if (fromDate != null && toDate <= DateTime.Now.AddMinutes(-60))
                {
                    var customers = customerJobMonitorService.GetCustInfoFromMainDBForMonitor(fromDate.Value, toDate.Value);
                    customers?.ToList().AddRange(customerJobMonitorService.GetCustSubInfoFromMainDBForMonitor(fromDate.Value, toDate.Value));

                    if (customers != null && customers.Any())
                    {
                        var missingCustomers = new List<MissingCustomerCompareEntity>();

                        var customerChunks = customers.Chunk(batchSize);

                        foreach (var customerBatch in customerChunks)
                        {
                            missingCustomers.AddRange(customerJobMonitorService.CompareCustomerDataForMonitor(customerBatch));
                        }

                        isSuccess = MissingCustomerProcess(new InsertMissingCustomerModel
                        {
                            CustomerInfo = customers,
                            MissingCustomers = missingCustomers,
                            IsAutoInsertMissing = isAutoInsertMissingCustomer,
                            ScannedTimeFrom = fromDate.Value,
                            ScannedTimeTo = toDate.Value
                        });
                    }

                    if (isSuccess)
                    {
                        systemParameterServices.UpdateTimingExecution(toDate.Value.ToString(CONVERT_TO_SQL_DATETIME), SystemParameters.JobMonitor_InsertNewCustomerLastScannedTime);
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

        public bool MissingCustomerProcess(InsertMissingCustomerModel newCustomers)
        {
            bool isSuccess = true;

            if (newCustomers.MissingCustomers.Any())
            {
                isSuccess = InsertMissingCustomer(newCustomers);

                if (isSuccess)
                {
                    InsertNullCategoryCustomer(newCustomers);
                }
            }

            return isSuccess;
        }

        private bool InsertMissingCustomer(InsertMissingCustomerModel newCustomers)
        {
            if (newCustomers.MissingCustomers.Any())
            {
                if (newCustomers.IsAutoInsertMissing)
                {
                    var data = new InsertCustomerViewModel
                    {
                        CustomerInfos = newCustomers.CustomerInfo.Where(c => newCustomers.MissingCustomers.Select(a => a.CustID).Contains(c.CustID)
                                                                        && newCustomers.MissingCustomers.Select(a => a.CustSubID).Contains(c.CustSubID))?.ToList()
                    };

                    return customerService.InsertNewCustomer(data, new InsertNewCustRescanEntity
                    {
                        InternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                        ExternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                        IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                        FeedSoon88API = appSettings.Soon88FeedDataAPI,
                        IsInsertMissingCust = true,
                        InsertVIPURL = appSettings.InsertVIPURL,
                        InsertBAURL = appSettings.InsertBAURL,
                        serviceName = serviceName
                    });
                }
                else
                {
                    customerJobMonitorService.InsertMonitorMissingCust(newCustomers.MissingCustomers, newCustomers.ScannedTimeTo);
                }

                NotifyTelegramMessage(newCustomers.MissingCustomers.Select(c => c.CustID), JobMonitorType.UnavailableNewCustomer, newCustomers.ScannedTimeFrom, newCustomers.ScannedTimeTo);
            }

            return true;
        }

        private void InsertNullCategoryCustomer(InsertMissingCustomerModel newCustomers)
        {
            if (newCustomers.MissingCustomers.Any())
            {
                var custToCheckCategory = newCustomers.CustomerInfo.Where(c => c.RoleID == 1 && !(newCustomers.MissingCustomers.Select(a => a.CustID).Contains(c.CustID)
                                                                                  && newCustomers.MissingCustomers.Select(a => a.CustSubID).Contains(c.CustSubID)));
                if (custToCheckCategory.Any())
                {
                    var nullCategoryCustomers = GetNullCategoryCustomers(custToCheckCategory.Select(c => (long)c.CustID).Distinct());

                    classificationJobMonitorService.InsertMonitorInvalidCustomerCategory(
                                nullCategoryCustomers.Select(c => new IncorrectCustomerCategoriesMonitorEntity { CustId = c, CategoryID = null })
                              , JobMonitorType.NewCustomerNULLCategory);

                    NotifyTelegramMessage(nullCategoryCustomers, JobMonitorType.NewCustomerNULLCategory, newCustomers.ScannedTimeFrom, newCustomers.ScannedTimeTo);
                }
            }
        }

        private IEnumerable<long> GetNullCategoryCustomers(IEnumerable<long> custIds)
        {
            if (custIds != null & custIds.Any())
            {
                var custCategories = classificationJobMonitorService.GetCustomerCategoriesToMonitor(custIds);

                var nullCategories = custCategories.Where(c => c.CurrentCategoryID == null);

                return nullCategories.Any() ? nullCategories.Select(c => c.CustId) : Enumerable.Empty<long>();
            }

            return Enumerable.Empty<long>();
        }

        private void NotifyTelegramMessage(IEnumerable<long> custIds, JobMonitorType type, DateTime scannedTimeFrom, DateTime scannedTimeTo)
        {
            if (custIds.Any())
            {
                var message = string.Empty;

                switch (type)
                {
                    case JobMonitorType.NewCustomerNULLCategory:
                        {
                            message = $"Please kindly check the following custid with category NULL - {{Total custid: {custIds.Distinct().Count()}}} - {string.Join(',', custIds.Distinct().Take(50))} - " +
                                      $"Scan from  \"{scannedTimeFrom:dd/MM/yyyy HH:mm:ss}\" to  \"{scannedTimeTo:dd/MM/yyyy HH:mm:ss}\"";
                            break;
                        }
                    case JobMonitorType.UnavailableNewCustomer:
                        {
                            message = $"Please kindly check the following custid with not available on CTS - {{Total custid: {custIds.Distinct().Count()}}} - {string.Join(',', custIds.Distinct().Take(50))} - " +
                                      $"Scan from \"{scannedTimeFrom:dd/MM/yyyy HH:mm:ss}\" to \"{scannedTimeTo:dd/MM/yyyy HH:mm:ss}\"";
                            break;
                        }
                }

                Utilities.NotifyTelegramMessage(message, "Monitoring New Customer", appSettings.EvironmentName, "0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", appSettings.EvironmentName == "LOCAL" ? TelegramGroupId_Local : TelegramGroupId_Pro);
            }
        }
    }
}