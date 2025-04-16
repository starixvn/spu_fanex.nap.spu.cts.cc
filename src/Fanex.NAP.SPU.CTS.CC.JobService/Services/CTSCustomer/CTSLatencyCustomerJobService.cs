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
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer;
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

        #region ScanLatencyCustomer
        public Task<bool> ScanLatencyCustomer(int externalBatchSize, int internalBatchSize, int scanSecondTime)
        {
            try
            {
                var isSuccess = true;

                var fromCustID = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.ScanLatencyCustomer_FromCustID);
                var toCustID = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.CTSCustomer_LastInsertCustID);
                var nextFromCustID = _missingCustomerService.GetLastScannedCustIDByScannedSecondTime((int)fromCustID, (int)toCustID, scanSecondTime);

                var customers = new List<int>();

                do
                {
                    var scanCustomer = _missingCustomerService.ScanLatencyCustomer((int)fromCustID, (int)toCustID, externalBatchSize);
                    fromCustID = scanCustomer.NextScannedCustID;

                    if (customers.IsNotNullOrEmpty())
                    {
                        foreach (var customerWithBatch in customers.Chunk(internalBatchSize))
                        {
                            var missingCusts = _missingCustomerService.CheckMissingCustomer(customerWithBatch);
                            _missingCustomerService.InsertMissingCustomerByLatencyToQueue(missingCusts);
                        }
                    }

                } while (customers.IsNotNullOrEmpty());

                _systemParameterServices.UpdateTimingExecution(nextFromCustID.ToString(), CTS_SystemParameters.ScanLatencyCustomer_FromCustID);

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
        #endregion

        #region InsertLatencyCustomer
        public Task<bool> InsertLatencyCustomer(int batchSize)
        {
            try
            {
                var isSuccess = true;

                var customers = _missingCustomerService.GetLatencyCustomerFromQueue(batchSize);

                var completeCustomers = Enumerable.Empty<int>();

                if (customers.IsNotNullOrEmpty())
                {
                    var custInfoCheckMissing = _missingCustomerService.CheckCustomerInfoFromMainDB(customers);

                    isSuccess = _insertNewCustomerService.InsertNewCustomer(true, custInfoCheckMissing, new InsertNewCustRescanEntity
                    {
                        InternalBatchSize = batchSize,
                        ExternalBatchSize = batchSize,
                        IsWriteLog = _appSettings.TransformCTSCustomerIsWriteLog,
                        IsInsertMissingCust = false,
                        InsertVIPURL = _appSettings.InsertVIPURL,
                        InsertBAURL = _appSettings.InsertBAURL,
                        serviceName = "InsertLatencyCustomer"
                    });

                    completeCustomers = custInfoCheckMissing.CustomerInfos?.Select(x => x.CustID);
                }

                if (isSuccess)
                {
                    _missingCustomerService.CompleteLatencyCustomerFromQueue(completeCustomers);
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
        #endregion
    }
}
