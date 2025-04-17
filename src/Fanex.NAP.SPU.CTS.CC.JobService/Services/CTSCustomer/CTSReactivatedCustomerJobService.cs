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
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models.ArchiveCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer;

    public interface ICTSReactivatedCustomerJobService
    {
        Task<bool> ScanReactivatedLatencyCustomers(int batchSize, int scannedSecondTime);

        Task<bool> InsertReactivatedCustomers();
    }

    public class CTSReactivatedCustomerJobService : ICTSReactivatedCustomerJobService
    {
        private readonly ICTSCustomerService _ctsCustomerService;
        private readonly IMissingCustomerService _missingCustomerService;
        private readonly IInsertNewCustomerService _insertNewCustomerService;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly ISentryClient _sentryClient;
        private readonly IAppSettings _appSettings;

        public CTSReactivatedCustomerJobService(
            ICTSCustomerService ctsCustomerService,
            IMissingCustomerService missingCustomerService,
            ISystemParameterServices systemParameterServices,
            IInsertNewCustomerService insertNewCustomerService,
            ICustClassToMainDBService custClassToMainDBService,
            ISentryClient sentryClient,
            IAppSettings appSettings)
        {
            _ctsCustomerService = ctsCustomerService;
            _missingCustomerService = missingCustomerService;
            _systemParameterServices = systemParameterServices;
            _insertNewCustomerService = insertNewCustomerService;
            _custClassToMainDBService = custClassToMainDBService;
            _sentryClient = sentryClient;
            _appSettings = appSettings;
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
                        _missingCustomerService.InsertMissingCustomerByLatencyToQueue(missingCustomers, JobMonitor_IssueType.ReactivatedCustomer_Missing);
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

        #region InsertReactivatedCustomers
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public Task<bool> InsertReactivatedCustomers()
        {
            try
            {
                var isSuccess = true;

                var lastScannedTime = _systemParameterServices.GetLastScannedDateTime(CTS_SystemParameters.ReactivatedCustomerLastScannedDate);

                var maxLogTime = DateTime.MaxValue;
                var custInfoTime = DateTime.MaxValue;
                var requestTime = DateTime.Now;

                var custToReactive = _ctsCustomerService.GetReactivatedCustomers(lastScannedTime, null);

                var ctsArchiveCustIDs = CheckExistCTSArchiveCustomerAndPushCC(custToReactive);

                custInfoTime = DateTime.Now;

                if (ctsArchiveCustIDs.IsPushCCSuccess)
                {
                    var isInsertReactivedCust = InsertNewCustFromReactivedCust(ctsArchiveCustIDs.NotExistCusts);

                    if (isInsertReactivedCust)
                    {
                        maxLogTime = custToReactive.Select(c => c.LogTime).Max();
                        isSuccess = _systemParameterServices.UpdateTimingExecution(custToReactive.Select(c => c.LogTime).Max().ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.ReactivatedCustomerLastScannedDate);
                    }
                }

                _ctsCustomerService.InsertReactivedCustomerLog(lastScannedTime, requestTime, (maxLogTime != DateTime.MaxValue) ? maxLogTime : null, (custInfoTime != DateTime.MaxValue) ? custInfoTime : null);

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

        #region Private Methods InsertReactivatedCustomers
        private CheckExistCTSArchiveCustModel CheckExistCTSArchiveCustomerAndPushCC(IEnumerable<ReactivatedCustModel> reactivedCusts)
        {
            if (reactivedCusts.IsNullOrEmpty())
            {
                return new CheckExistCTSArchiveCustModel();
            }

            var exitedCTSArchiveCustIDs = _ctsCustomerService.CheckExistCTSArchiveCustomer(reactivedCusts);

            return new CheckExistCTSArchiveCustModel
            {
                ExistCusts = exitedCTSArchiveCustIDs,
                NotExistCusts = reactivedCusts.Select(x => x.CustID).Except(exitedCTSArchiveCustIDs),
                IsPushCCSuccess = PushDataToExternal(exitedCTSArchiveCustIDs, "InsertReactivatedCustomers")
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        private bool InsertNewCustFromReactivedCust(IEnumerable<long> insertNewCusts)
        {
            if (insertNewCusts.IsNullOrEmpty())
            {
                return true;
            }

            // sleep for latency
            Thread.Sleep(2000);

            var custInfo = GetMissingCustInfo(insertNewCusts);

            return _insertNewCustomerService.InsertNewCustomer(false, custInfo, new InsertNewCustRescanEntity
            {
                InternalBatchSize = _appSettings.TransformCTSCustomerBatchSize,
                ExternalBatchSize = _appSettings.TransformCTSCustomerBatchSize,
                IsWriteLog = _appSettings.TransformCTSCustomerIsWriteLog,
                IsInsertMissingCust = false,
                InsertVIPURL = _appSettings.InsertVIPURL,
                InsertBAURL = _appSettings.InsertBAURL,
                serviceName = "ReactivatedCustomer"
            });
        }

        private InsertCustomerModel GetMissingCustInfo(IEnumerable<long> custIds)
        {
            var custInfo = new InsertCustomerModel { CustomerInfos = new List<CustomerMainDBEntity>(), LastInsertCustID = null };

            if (custIds != null && custIds.Any())
            {
                var custToString = string.Join(",", custIds.Select(c => c).Distinct());
                var mainCusts = _missingCustomerService.CheckCustomerInfoFromMainDB(custIds.Distinct().Select(x => (int)x));
                var subCusts = _ctsCustomerService.GetCustSubFromMainDBByCustIDs(custToString);

                custInfo.CustomerInfos.AddRange(mainCusts.CustomerInfos);
                custInfo.CustomerInfos.AddRange(subCusts);
            }

            return custInfo;
        }

        private bool PushDataToExternal(IEnumerable<long> custIDs, string serviceName)
        {
            return _custClassToMainDBService.PushExternalPackageByCustId(custIDs.Distinct(), new PushDataToExternalEntity
            {
                RoleGroup = RoleGroupType.Member,
                ExternalBatchSize = _appSettings.InsertReactivatedCustomersBatchSize,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = _appSettings.InsertReactivatedCustomersIsWriteLog,
                ServiceName = serviceName
            });
        }
        #endregion Private Methods

        #endregion
    }
}
