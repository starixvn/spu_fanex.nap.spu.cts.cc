namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
    using Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerJobMonitorJobService
    {
        Task<bool> ScanMissingCustomer(int internalBatchSize, int externalBatchSize, int addSecond);

        Task<bool> ScanMissingReactivatedCustomer(int batchSize, int addSecond);

        Task<bool> ScanIncorrectCustomerInfo(int batchSize);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
    public class CustomerJobMonitorJobService : ICustomerJobMonitorJobService
    {
        //private readonly string serviceName;

        private readonly ICTSCustomerService _ctscustomerService;
        private readonly IIncorrectDataMonitoringService _incorrectDataMonitoringService;
        private readonly ICustomerJobMonitorService _customerJobMonitorService;
        private readonly IMissingCustomerService _missingCustomerService;
        private readonly ISystemParameterServices _systemParameterServices;

        public CustomerJobMonitorJobService(

            IIncorrectDataMonitoringService incorrectDataMonitoringService,
            ICustomerJobMonitorService customerJobMonitorService,
            ISystemParameterServices systemParameterServices,
            ICTSCustomerService ctscustomerService,
            IMissingCustomerService missingCustomerService)
        {
            _incorrectDataMonitoringService = incorrectDataMonitoringService;
            _customerJobMonitorService = customerJobMonitorService;
            _systemParameterServices = systemParameterServices;
            _ctscustomerService = ctscustomerService;
            _missingCustomerService = missingCustomerService;
            //this.serviceName = "CustomerJobMonitor";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S134:Control flow statements \"if\", \"switch\", \"for\", \"foreach\", \"while\", \"do\"  and \"try\" should not be nested too deeply", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S103:Lines should not be too long", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2971:LINQ expressions should be simplified", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        public Task<bool> ScanMissingCustomer(int internalBatchSize, int externalBatchSize, int addSecond)
        {
            try
            {
                var fromCustID = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.JobMonitor_NewCustomerLastScannedCustID);
                var lastCustID = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.CTSCustomer_LastInsertCustID);
                var toCustID = _missingCustomerService.GetLastScannedCustIDByScannedSecondTime(
                    (int)fromCustID, 
                    (int)lastCustID, 
                    addSecond);

                if (toCustID.HasValue)
                {
                    var scanCustomer = default(ScanLatencyCustomerModel);

                    do
                    {
                        scanCustomer = _missingCustomerService.ScanLatencyCustomer((int)fromCustID, (int)toCustID, externalBatchSize);

                        if (scanCustomer.ScanLatencyCustomers.IsNotNullOrEmpty() && scanCustomer.NextScannedCustID.HasValue)
                        {
                            fromCustID = scanCustomer.NextScannedCustID.Value;

                            var missingCustomers = new List<int>();

                            foreach (var customerWithBatch in scanCustomer.ScanLatencyCustomers.Chunk(internalBatchSize))
                            {
                                var missingCusts = _missingCustomerService.CheckMissingCustomer(customerWithBatch.Select(x => x.CustID));
                                missingCustomers.AddRange(missingCusts);
                            }

                            MonitorMissingCustomers(missingCustomers, JobMonitor_IssueType.NewCustomer_Missing, internalBatchSize);
                        }

                    } while (scanCustomer.ScanLatencyCustomers.IsNotNullOrEmpty());

                    _systemParameterServices.UpdateTimingExecution(toCustID.ToString(), CTS_SystemParameters.JobMonitor_NewCustomerLastScannedCustID);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                logMessage.LogError();

                return Task.FromResult(false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public Task<bool> ScanMissingReactivatedCustomer(int batchSize, int addSecond)
        {
            try
            {
                var fromDate = _systemParameterServices.GetLastScannedDateTime(CTS_SystemParameters.JobMonitoring_ReactivatedCustomerLastScannedTime);
                var toDate = fromDate?.AddSeconds(addSecond);

                if (fromDate != null && toDate <= DateTime.Now.AddSeconds(-addSecond))
                {
                    var custToReactive = _ctscustomerService.GetReactivatedCustomers(fromDate, toDate);

                    if (custToReactive.IsNotNullOrEmpty())
                    {
                        var missingCustomers = new List<int>();

                        foreach (var custToReactiveWithBatch in custToReactive.Chunk(batchSize))
                        {
                            var missingCust = _missingCustomerService.CheckMissingCustomer(custToReactiveWithBatch.Select(x => (int)x.CustID));
                            missingCustomers.AddRange(missingCust);
                        }

                        MonitorMissingCustomers(missingCustomers, JobMonitor_IssueType.ReactivatedCustomer_Missing, batchSize);
                    }

                    _systemParameterServices.UpdateTimingExecution(toDate.Value.ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.JobMonitoring_ReactivatedCustomerLastScannedTime);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                logMessage.LogError();
                return Task.FromResult(false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S134:Control flow statements \"if\", \"switch\", \"for\", \"foreach\", \"while\", \"do\"  and \"try\" should not be nested too deeply", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S103:Lines should not be too long", Justification = "<Pending>")]
        public Task<bool> ScanIncorrectCustomerInfo(int batchSize)
        {
            try
            {
                var lastCTSCustId = _systemParameterServices.GetLastScannedId2(CTS_SystemParameters.JobMonitoring_NewCustomer_LastCTSCustID);

                if (lastCTSCustId > 0)
                {
                    var result = _customerJobMonitorService.CheckIncorrectCustomerInfo(lastCTSCustId);

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
                            _systemParameterServices.UpdateTimingExecution(result.NextScannedCTSCustId.ToString(), CTS_SystemParameters.JobMonitoring_NewCustomer_LastCTSCustID);
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

                logMessage.LogError();

                return Task.FromResult(false);
            }
        }

        #region Private Methods
        private void MonitorMissingCustomers(IEnumerable<int> missingsCustomer, JobMonitor_IssueType missingCustomerIssueType, int batchSize)
        {
            if (missingsCustomer.IsNotNullOrEmpty())
            {
                _incorrectDataMonitoringService.InsertIncorrectDataMonitoring(missingsCustomer.Distinct().Select(x => (long)x), missingCustomerIssueType, batchSize);
            }
        }

        private void InsertIncorrectCustomerInfo(IEnumerable<long> missingCustomerInfo, int batchSize)
        {
            if (missingCustomerInfo.Any())
            {
                _incorrectDataMonitoringService.InsertIncorrectDataMonitoring(missingCustomerInfo.Distinct(), JobMonitor_IssueType.NewCustomer_IncorrectCustInfo, batchSize);
            }
        }
        #endregion Private Methods
    }
}