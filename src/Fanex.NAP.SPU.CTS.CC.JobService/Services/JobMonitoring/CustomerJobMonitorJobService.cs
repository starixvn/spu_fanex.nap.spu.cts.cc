namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
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
        Task<bool> ScanMissingCustomer(int batchSize, int externalSize);

        Task<bool> ScanMissingCustomerInfo(int batchSize);
    }

    public class CustomerJobMonitorJobService : ICustomerJobMonitorJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IIncorrectDataMonitoringService incorrectDataMonitoringService;
        private readonly ICustomerJobMonitorService customerJobMonitorService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly IAppSettings appSettings;

        public CustomerJobMonitorJobService(
            ISentryClient sentryClient,
            IIncorrectDataMonitoringService incorrectDataMonitoringService,
            ICustomerJobMonitorService customerJobMonitorService,
            ISystemParameterServices systemParameterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.incorrectDataMonitoringService = incorrectDataMonitoringService;
            this.customerJobMonitorService = customerJobMonitorService;
            this.systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
            this.serviceName = "CustomerJobMonitor";
        }

        public Task<bool> ScanMissingCustomer(int batchSize, int externalSize)
        {
            try
            {
                var fromDate = systemParameterServices.GetLastScannedDateTime(SystemParameters.JobMonitor_NewCustomerLastScannedTime);
                var toDate = fromDate?.AddMinutes(60);
                bool isSuccess = true;

                if (fromDate != null && toDate <= DateTime.Now.AddMinutes(-60))
                {
                    var lastScannedCustID = systemParameterServices.GetLastScannedId(SystemParameters.JobMonitor_NewCustomerLastScannedCustID);
                    var lastScannedCustSubID = systemParameterServices.GetLastScannedId(SystemParameters.JobMonitor_NewCustomerLastScannedCustSubID);

                    var mainCusts = customerJobMonitorService.GetCustInfoFromMainDBForMonitor(externalSize, toDate.Value, lastScannedCustID);
                    var custSubs = customerJobMonitorService.GetCustSubInfoFromMainDBForMonitor(externalSize, toDate.Value, lastScannedCustSubID);
                    var customers = mainCusts.NewCustomers?.ToList().Concat(custSubs.NewCustomers);

                    if (customers != null && customers.Any())
                    {
                        var missingCustomers = new List<MissingCustomerCompareEntity>();

                        var customerChunks = customers.Chunk(batchSize);

                        foreach (var custBatch in customerChunks)
                        {
                            missingCustomers.AddRange(customerJobMonitorService.CheckMissingCustomer(custBatch));
                        }

                        MonitorNewCustomers(new InsertMissingCustomerModel
                        {
                            CustomerInfo = customers,
                            MissingCustomers = missingCustomers
                        }, batchSize);

                        UpdateLastScanCustomer(mainCusts.MaxScannedCustomerID, custSubs.MaxScannedCustomerID);
                    }

                    if (customers != null && !customers.Any() && lastScannedCustID != 0 && lastScannedCustSubID != 0)
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

        public Task<bool> ScanMissingCustomerInfo(int batchSize)
        {
            try
            {
                var lastCTSCustId = systemParameterServices.GetLastScannedId2(SystemParameters.JobMonitoring_NewCustomer_LastCTSCustID);
                bool isSuccess = true;

                if (lastCTSCustId > 0)
                {
                    var result = customerJobMonitorService.CheckMissingCustomerInfo(lastCTSCustId);

                    if (result != null)
                    {
                        if (result.CustomerInfo != null && result.CustomerInfo.Any())
                        {
                            InsertMissingCustomerInfo(result.CustomerInfo, batchSize);
                        }

                        if (result.NextScannedCTSCustId != null && result.NextScannedCTSCustId > 0)
                        {
                            systemParameterServices.UpdateTimingExecution(result.NextScannedCTSCustId.ToString(), SystemParameters.JobMonitoring_NewCustomer_LastCTSCustID);
                        }
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

        #region Private Methods

        private void MonitorNewCustomers(InsertMissingCustomerModel newCustomers, int batchSize)
        {
            if (newCustomers.MissingCustomers.Any())
            {
                InsertMissingCustomer(newCustomers, batchSize);
            }

            InsertNullCategoryCustomer(newCustomers, batchSize);
        }

        private void InsertMissingCustomer(InsertMissingCustomerModel newCustomers, int batchSize)
        {
            if (newCustomers.MissingCustomers.Any())
            {
                var missingCustomers = newCustomers.MissingCustomers.Select(c => c.CustID).Distinct();

                incorrectDataMonitoringService.InsertIncorrectDataMonitoring(missingCustomers, JobMonitor_IssueType.NewCustomer_Missing, batchSize);
            }
        }

        private void InsertMissingCustomerInfo(IEnumerable<long> missingCustomerInfo, int batchSize)
        {
            if (missingCustomerInfo.Any())
            {
                incorrectDataMonitoringService.InsertIncorrectDataMonitoring(missingCustomerInfo.Distinct(), JobMonitor_IssueType.NewCustomer_MissingCustInfo, batchSize);
            }
        }

        private void InsertNullCategoryCustomer(InsertMissingCustomerModel newCustomers, int batchSize)
        {
            if (newCustomers.CustomerInfo.Any())
            {
                var custToCheckCategory = newCustomers.CustomerInfo.Where(c => c.RoleID == 1
                                                                           && !(newCustomers.MissingCustomers.Select(a => a.CustID).Contains(c.CustID)
                                                                           && newCustomers.MissingCustomers.Select(a => a.CustSubID).Contains(c.CustSubID)));
                if (custToCheckCategory.Any())
                {
                    var nullCategory = GetNullCategoryCustomers(custToCheckCategory.Select(c => (long)c.CustID).Distinct(), batchSize);

                    incorrectDataMonitoringService.InsertIncorrectDataMonitoring(nullCategory, JobMonitor_IssueType.NewCustomer_NullCategory, batchSize);
                }
            }
        }

        private IEnumerable<long> GetNullCategoryCustomers(IEnumerable<long> custIds, int batchSize)
        {
            if (custIds != null & custIds.Any())
            {
                var custCategories = customerJobMonitorService.GetCustomerCategoryToMonitor(custIds, batchSize);

                var nullCategories = custCategories.Where(c => c.CurrentCategoryID == null);

                return nullCategories.Any() ? nullCategories.Select(c => c.CustID) : Enumerable.Empty<long>();
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

        #endregion Private Methods
    }
}