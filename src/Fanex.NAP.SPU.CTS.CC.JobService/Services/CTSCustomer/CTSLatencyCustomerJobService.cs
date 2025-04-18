namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Shares;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSLatencyCustomerJobService
    {
        Task<bool> ScanLatencyCustomer(int externalBatchSize, int internalBatchSize, int scanSecondTime);

        Task<bool> InsertLatencyCustomer(int batchSize);
    }

    public class CTSLatencyCustomerJobService : ICTSLatencyCustomerJobService
    {
        private readonly IMissingCustomerService _missingCustomerService;
        private readonly IInsertNewCustomerService _insertNewCustomerService;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly IAppSettings _appSettings;
        private readonly ISentryClient _sentryClient;

        public CTSLatencyCustomerJobService(
            IMissingCustomerService missingCustomerService,
            IInsertNewCustomerService insertNewCustomerService,
            ISystemParameterServices systemParameterServices,
            IAppSettings appSettings,
            ISentryClient sentryClient)
        {
            _missingCustomerService = missingCustomerService;
            _insertNewCustomerService = insertNewCustomerService;
            _systemParameterServices = systemParameterServices;
            _appSettings = appSettings;
            _sentryClient = sentryClient;
        }



        #region ScanLatencyCustomer
        public Task<bool> ScanLatencyCustomer(int externalBatchSize, int internalBatchSize, int scanSecondTime)
        {
            try
            {
                var fromCustID = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.ScanLatencyCustomer_FromCustID);
                var lastCustID = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.CTSCustomer_LastInsertCustID);
                var toCustID = _missingCustomerService.GetLastScannedCustIDByScannedSecondTime(
                    (int)fromCustID,
                    (int)lastCustID,
                    scanSecondTime);

                if (toCustID.HasValue)
                {
                    ScanAndProcessLatencyCustomers(
                        (int)fromCustID,
                        (int)lastCustID,
                        externalBatchSize,
                        internalBatchSize);

                    _systemParameterServices.UpdateTimingExecution(
                        toCustID.Value.ToString(),
                        CTS_SystemParameters.ScanLatencyCustomer_FromCustID);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
                  .ToString();

                _sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        private void ScanAndProcessLatencyCustomers(int fromCustID, int toCustID, int externalBatchSize, int internalBatchSize)
        {
            var scanResult = default(ScanLatencyCustomerEntity);

            do
            {
                scanResult = _missingCustomerService.ScanLatencyCustomer(fromCustID, toCustID, externalBatchSize);

                if (scanResult.CustIDs.IsNotNullOrEmpty() && scanResult.NextScannedCustID.HasValue)
                {
                    fromCustID = scanResult.NextScannedCustID.Value;
                    ProcessCustomersInBatches(scanResult.CustIDs, internalBatchSize);
                }

            } while (scanResult.CustIDs.IsNotNullOrEmpty());
        }

        private void ProcessCustomersInBatches(IEnumerable<int> customers, int batchSize)
        {
            foreach (var customerBatch in customers.Chunk(batchSize))
            {
                var missingCustomers = _missingCustomerService.CheckMissingCustomer(customerBatch);
                _missingCustomerService.InsertMissingCustomerByLatencyToQueue(
                    missingCustomers,
                    JobMonitor_IssueType.NewCustomer_Missing);
            }
        }
        #endregion

        #region InsertLatencyCustomer
        public Task<bool> InsertLatencyCustomer(int batchSize)
        {
            try
            {
                var customers = _missingCustomerService.GetLatencyCustomerFromQueue(batchSize);

                if (customers.IsNullOrEmpty())
                {
                    return Task.FromResult(true);
                }

                var newCustomerIds = customers
                    .Where(BusinessExtension.MatchLatencyNewCustomer())
                    .Select(x => x.CustID)
                    .ToArray();

                var reactivatedCustomerIds = customers
                    .Where(BusinessExtension.MatchLatencyReactivatedCustomer())
                    .Select(x => x.CustID)
                    .ToArray();

                var newCustomerResult = (Enumerable.Empty<int>(), true);
                var reactivatedCustomerResult = (Enumerable.Empty<int>(), true);

                Parallel.Invoke(
                    () => newCustomerResult = CheckInfoAndInsertCustomerToCTSSystem(true, batchSize, newCustomerIds),
                    () => reactivatedCustomerResult = CheckInfoAndInsertCustomerToCTSSystem(false, batchSize, reactivatedCustomerIds)
                );

                var isSuccess = newCustomerResult.Item2 && reactivatedCustomerResult.Item2;

                if (isSuccess)
                {
                    var processedCustomerIds = newCustomerResult.Item1.Concat(reactivatedCustomerResult.Item1).ToList();
                    _missingCustomerService.CompleteLatencyCustomerFromQueue(processedCustomerIds);
                }

                return Task.FromResult(isSuccess);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
                  .ToString();

                _sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        private (IEnumerable<int>, bool) CheckInfoAndInsertCustomerToCTSSystem(bool isNewCustomer, int batchSize, IEnumerable<int> customerIds)
        {
            if (customerIds.IsNullOrEmpty())
            {
                return (Enumerable.Empty<int>(), true);
            }

            var customerInfo = _missingCustomerService.CheckCustomerInfoFromMainDB(customerIds);

            var insertConfig = new InsertNewCustRescanEntity
            {
                InternalBatchSize = batchSize,
                ExternalBatchSize = batchSize,
                IsWriteLog = _appSettings.TransformCTSCustomerIsWriteLog,
                IsInsertMissingCust = false,
                InsertVIPURL = _appSettings.InsertVIPURL,
                InsertBAURL = _appSettings.InsertBAURL,
                serviceName = "InsertLatencyCustomer"
            };

            var isSuccess = _insertNewCustomerService.InsertNewCustomer(isNewCustomer, customerInfo, insertConfig);
            var processedCustomerIds = customerInfo.CustomerInfos?.Select(x => x.CustID) ?? Enumerable.Empty<int>();

            return (processedCustomerIds, isSuccess);
        }
        #endregion
    }
}
