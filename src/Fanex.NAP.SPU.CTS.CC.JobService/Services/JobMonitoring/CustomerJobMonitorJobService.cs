namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
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
        Task<bool> ScanMissingCustomer(int batchSize, int externalSize, int addMinute);

        Task<bool> ScanMissingReactivatedCustomer(int batchSize, int addMinute);

        Task<bool> ScanIncorrectCustomerInfo(int batchSize);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
    public class CustomerJobMonitorJobService : ICustomerJobMonitorJobService
    {
        //private readonly string serviceName;

        private readonly ICTSCustomerService ctscustomerService;
        private readonly ISentryClient sentryClient;
        private readonly IIncorrectDataMonitoringService incorrectDataMonitoringService;
        private readonly ICustomerJobMonitorService customerJobMonitorService;
        private readonly ISystemParameterServices systemParameterServices;

        public CustomerJobMonitorJobService(
            ISentryClient sentryClient,
            IIncorrectDataMonitoringService incorrectDataMonitoringService,
            ICustomerJobMonitorService customerJobMonitorService,
            ISystemParameterServices systemParameterServices,
            ICTSCustomerService ctscustomerService)
        {
            this.sentryClient = sentryClient;
            this.incorrectDataMonitoringService = incorrectDataMonitoringService;
            this.customerJobMonitorService = customerJobMonitorService;
            this.systemParameterServices = systemParameterServices;
            this.ctscustomerService = ctscustomerService;
            //this.serviceName = "CustomerJobMonitor";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S134:Control flow statements \"if\", \"switch\", \"for\", \"foreach\", \"while\", \"do\"  and \"try\" should not be nested too deeply", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S103:Lines should not be too long", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2971:LINQ expressions should be simplified", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        public Task<bool> ScanMissingCustomer(int batchSize, int externalSize, int addMinute)
        {
            try
            {
                int addTime = (addMinute == 0) ? 15 : addMinute;
                var fromDate = systemParameterServices.GetLastScannedDateTime(CTS_SystemParameters.JobMonitor_NewCustomerLastScannedTime);
                var toDate = fromDate?.AddMinutes(addMinute);

                if (fromDate != null && toDate <= DateTime.Now.AddMinutes(-addMinute))
                {
                    var lastScannedCustID = systemParameterServices.GetLastScannedId(CTS_SystemParameters.JobMonitor_NewCustomerLastScannedCustID);
                    var lastScannedCustSubID = systemParameterServices.GetLastScannedId(CTS_SystemParameters.JobMonitor_NewCustomerLastScannedCustSubID);

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
                            MissingCustomers = missingCustomers,
                            MissingCustomerIssueType = JobMonitor_IssueType.NewCustomer_Missing
                        }, batchSize);

                        UpdateLastScanCustomer(mainCusts.MaxScannedCustomerID, custSubs.MaxScannedCustomerID);
                    }

                    if (customers != null && !customers.Any() && lastScannedCustID != 0 && lastScannedCustSubID != 0)
                    {
                        systemParameterServices.UpdateTimingExecution(toDate.Value.ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.JobMonitor_NewCustomerLastScannedTime);
                    }
                }

                return Task.FromResult(true);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public Task<bool> ScanMissingReactivatedCustomer(int batchSize, int addMinute)
        {
            try
            {
                int addTime = (addMinute == 0) ? 60 : addMinute;
                var fromDate = systemParameterServices.GetLastScannedDateTime(CTS_SystemParameters.JobMonitoring_ReactivatedCustomerLastScannedTime);
                var toDate = fromDate?.AddMinutes(addMinute);

                if (fromDate != null && toDate != null && toDate <= DateTime.Now.AddMinutes(-addMinute))
                {
                    var custToReactive = ctscustomerService.GetReactivatedCustomers(fromDate, toDate);

                    if (custToReactive != null && custToReactive.Any())
                    {
                        var missingCustomers = new List<MissingCustomerCompareEntity>();

                        var customerChunks = custToReactive.Chunk(batchSize);

                        foreach (var custBatch in customerChunks)
                        {
                            var input = custBatch.Select(x => new MonitoringNewCustomerEntity(x));

                            missingCustomers.AddRange(customerJobMonitorService.CheckMissingCustomer(input));
                        }

                        MonitorNewCustomers(new InsertMissingCustomerModel
                        {
                            CustomerInfo = Enumerable.Empty<MonitoringNewCustomerEntity>(),
                            MissingCustomers = missingCustomers,
                            MissingCustomerIssueType = JobMonitor_IssueType.ReactivatedCustomer_Missing
                        }, batchSize);
                    }

                    systemParameterServices.UpdateTimingExecution(toDate.Value.ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.JobMonitoring_ReactivatedCustomerLastScannedTime);
                }

                return Task.FromResult(true);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S134:Control flow statements \"if\", \"switch\", \"for\", \"foreach\", \"while\", \"do\"  and \"try\" should not be nested too deeply", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S103:Lines should not be too long", Justification = "<Pending>")]
        public Task<bool> ScanIncorrectCustomerInfo(int batchSize)
        {
            try
            {
                var lastCTSCustId = systemParameterServices.GetLastScannedId2(CTS_SystemParameters.JobMonitoring_NewCustomer_LastCTSCustID);

                if (lastCTSCustId > 0)
                {
                    var result = customerJobMonitorService.CheckIncorrectCustomerInfo(lastCTSCustId);

                    if (result != null)
                    {
                        if (result.CustomerInfo != null && result.CustomerInfo.Any())
                        {
                            InsertIncorrectCustomerInfo(result.CustomerInfo, batchSize);
                        }

                        if (result.NextScannedCTSCustId != null
                            && result.NextScannedCTSCustId > 0
                            && result.NextScannedCTSCustId > lastCTSCustId)
                        {
                            systemParameterServices.UpdateTimingExecution(result.NextScannedCTSCustId.ToString(), CTS_SystemParameters.JobMonitoring_NewCustomer_LastCTSCustID);
                        }
                    }
                }

                return Task.FromResult(true);
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

                incorrectDataMonitoringService.InsertIncorrectDataMonitoring(missingCustomers, newCustomers.MissingCustomerIssueType, batchSize);
            }
        }

        private void InsertIncorrectCustomerInfo(IEnumerable<long> missingCustomerInfo, int batchSize)
        {
            if (missingCustomerInfo.Any())
            {
                incorrectDataMonitoringService.InsertIncorrectDataMonitoring(missingCustomerInfo.Distinct(), JobMonitor_IssueType.NewCustomer_IncorrectCustInfo, batchSize);
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
            if (custIds != null && custIds.Any())
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
                systemParameterServices.UpdateTimingExecution(maxScannedCustId.ToString(), CTS_SystemParameters.JobMonitor_NewCustomerLastScannedCustID);
            }

            if (maxScannedCustSubId != null)
            {
                systemParameterServices.UpdateTimingExecution(maxScannedCustSubId.ToString(), CTS_SystemParameters.JobMonitor_NewCustomerLastScannedCustSubID);
            }
        }

        #endregion Private Methods
    }
}