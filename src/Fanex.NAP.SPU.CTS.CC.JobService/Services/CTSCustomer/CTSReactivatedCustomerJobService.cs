namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer;
    using Fanex.NAP.SPU.CTS.Shares;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSReactivatedCustomerJobService
    {
        Task<bool> ScanReactivatedLatencyCustomers(int batchSize, int scannedSecondTime);
    }

    public class CTSReactivatedCustomerJobService : ICTSReactivatedCustomerJobService
    {
        private readonly ICTSCustomerService _ctsCustomerService;
        private readonly IMissingCustomerService _missingCustomerService;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly ISentryClient _sentryClient;

        public CTSReactivatedCustomerJobService(
            ICTSCustomerService ctsCustomerService, 
            IMissingCustomerService missingCustomerService, 
            ISystemParameterServices systemParameterServices, 
            ISentryClient sentryClient)
        {
            _ctsCustomerService = ctsCustomerService;
            _missingCustomerService = missingCustomerService;
            _systemParameterServices = systemParameterServices;
            _sentryClient = sentryClient;
        }

        public CTSReactivatedCustomerJobService(ISystemParameterServices systemParameterServices)
        {
            _systemParameterServices = systemParameterServices;
        }

        #region ScanReactivatedLatencyCustomers
        public Task<bool> ScanReactivatedLatencyCustomers(int batchSize, int scannedSecondTime)
        {
            try
            {
                var isSuccess = true;

                var lastLogTime = _systemParameterServices.GetLastScannedDateTime(CTS_SystemParameters.ScanReactivatedLatencyCustomer_LastLogTime);

                var reactivatedCustomers = _ctsCustomerService.GetReactivatedCustomers(lastLogTime, null);

                var updateNextLogTime = default(DateTime?);

                if (reactivatedCustomers.IsNotNullOrEmpty())
                {
                    var minLastLogTimeAfterScannedSecondTime = reactivatedCustomers.Min(x => x.LogTime).AddSeconds(-scannedSecondTime);

                    updateNextLogTime = (lastLogTime == null || minLastLogTimeAfterScannedSecondTime > lastLogTime)
                        ? minLastLogTimeAfterScannedSecondTime
                        : lastLogTime;

                    foreach (var reactivatedCustomersWithBatch in reactivatedCustomers.Chunk(batchSize))
                    {
                        var missingCustomers = _missingCustomerService.CheckMissingCustomer(reactivatedCustomersWithBatch.Select(x => (int)x.CustID));
                        _missingCustomerService.InsertMissingCustomerByLatencyToQueue(missingCustomers);
                    }
                }
                else
                {
                    updateNextLogTime = lastLogTime;
                }

                _systemParameterServices.UpdateTimingExecution(updateNextLogTime.ToString(), CTS_SystemParameters.ScanReactivatedLatencyCustomer_LastLogTime);

                return Task.FromResult(isSuccess);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                _sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }
        #endregion

    }
}
