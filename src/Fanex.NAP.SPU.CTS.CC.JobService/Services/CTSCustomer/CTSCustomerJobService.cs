namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models.ArchiveCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerJobService
    {
        Task<bool> InsertNewCustomer(int batchSize);

        Task<bool> ScanLatencyCustomer(int externalBatchSize, int internalBatchSize, int scanSecondTime);

        Task<bool> InsertLatencyCustomer(int batchSize);

        Task<bool> InsertMissingCustomer(int batchSize);

        Task<bool> UpdateCTSCustomerInfo(int packageSize, int waitTime);

        Task<bool> UpdateUsername2ForCustMain(int batchSize);

        Task<bool> UpdateUsername2ForCustSub(int batchSize);

        Task<bool> ScanReactivatedLatencyCustomers(int batchSize, int scannedSecondTime);

        Task<bool> InsertReactivatedCustomers();

        Task<bool> InactivateNewCustomers(int batchSize);
    }

    public class CTSCustomerJobService : ICTSCustomerJobService
    {
        private readonly ICTSCustomerService _ctsCustomerService;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public CTSCustomerJobService(ISentryClient sentryClient,
            ICTSCustomerService ctscustomerService,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IRescanCustClassificationService rescanCustClassificationService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            _ctsCustomerService = ctscustomerService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.rescanCustClassificationService = rescanCustClassificationService;
            this._systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
        }

        #region InsertNewCustomer
        public Task<bool> InsertNewCustomer(int batchSize)
        {
            try
            {
                var isSuccess = true;
                var customers = new InsertCustomerModel();
                var newSubCustomers = Enumerable.Empty<CustomerMainDBEntity>();

                var lastScanned = _ctsCustomerService.GetLastInsertCust();


                Parallel.Invoke(
                    () => { customers = GetNewCustomerFromMainDBAndHandleMissingCustomer(batchSize, lastScanned.LastCustID); },
                    () => { newSubCustomers = _ctsCustomerService.GetCustSubFromMainDBByLastCustSub(batchSize, lastScanned.LastCustSubID); });


                if (customers?.CustomerInfos != null)
                {
                    customers.CustomerInfos.AddRange(newSubCustomers);

                    isSuccess = _ctsCustomerService.InsertNewCustomer(true, customers, new InsertNewCustRescanEntity
                    {
                        InternalBatchSize = batchSize,
                        ExternalBatchSize = batchSize,
                        IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                        IsInsertMissingCust = false,
                        InsertVIPURL = appSettings.InsertVIPURL,
                        InsertBAURL = appSettings.InsertBAURL,
                        serviceName = "InsertNewCustomer"
                    });
                }

                return Task.FromResult(isSuccess);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        private InsertCustomerModel GetNewCustomerFromMainDBAndHandleMissingCustomer(int numOfAccount, Int64 lastScannedCustID)
        {
            var customers = _ctsCustomerService.GetCustomerFromMainDBForInsertNew(numOfAccount, lastScannedCustID);

            _ctsCustomerService.InsertMissingCustomerLatencyToQueue(customers.MissingCustIds);

            return customers;
        }
        #endregion

        #region ScanLatencyCustomer
        public Task<bool> ScanLatencyCustomer(int externalBatchSize, int internalBatchSize, int scanSecondTime)
        {
            try
            {
                var isSuccess = true;

                var fromCustID = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.ScanLatencyCustomer_FromCustID);
                var toCustID = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.CTSCustomer_LastInsertCustID);
                var nextFromCustID = _ctsCustomerService.GetLastScannedCustID((int)fromCustID, (int)toCustID, scanSecondTime);

                var customers = new List<int>();

                do
                {
                    var scanCustomer = _ctsCustomerService.ScanLatencyCustomer((int)fromCustID, (int)toCustID, externalBatchSize);
                    fromCustID = scanCustomer.NextScannedCustID;

                    if (customers.IsNotNullOrEmpty())
                    {
                        foreach (var customerWithBatch in customers.Chunk(internalBatchSize))
                        {
                            var missingCusts = _ctsCustomerService.CheckMissingCustomer(customerWithBatch);
                            _ctsCustomerService.InsertMissingCustomerLatencyToQueue(missingCusts);
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

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
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

                var customers = _ctsCustomerService.GetLatencyCustomerFromQueue(batchSize);

                var completeCustomers = Enumerable.Empty<int>();

                if (customers.IsNotNullOrEmpty())
                {
                    var custInfoCheckMissing = _ctsCustomerService.CheckCustomerInfoFromMainDB(customers);

                    isSuccess = _ctsCustomerService.InsertNewCustomer(true, custInfoCheckMissing, new InsertNewCustRescanEntity
                    {
                        InternalBatchSize = batchSize,
                        ExternalBatchSize = batchSize,
                        IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                        IsInsertMissingCust = false,
                        InsertVIPURL = appSettings.InsertVIPURL,
                        InsertBAURL = appSettings.InsertBAURL,
                        serviceName = "InsertLatencyCustomer"
                    });

                    completeCustomers = custInfoCheckMissing.CustomerInfos?.Select(x => x.CustID);
                }

                if (isSuccess)
                {
                    _ctsCustomerService.CompleteLatencyCustomerFromQueue(completeCustomers);
                }

                return Task.FromResult(isSuccess);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }
        #endregion

        public Task<bool> InsertMissingCustomer(int batchSize)
        {
            try
            {
                var isSuccess = true;
                var custInfo = new InsertCustomerModel();

                var data = _ctsCustomerService.GetMissingCustomers();

                if (data == null || !data.Any())
                {
                    return Task.FromResult(isSuccess);
                }

                var missingCusts = data.Where(c => c.IsMissingCust);
                var notMissingCusts = data.Where(c => !c.IsMissingCust);

                if (missingCusts.Any())
                {
                    var chunkCusts = missingCusts.Distinct().Chunk(batchSize);

                    foreach (var batchCust in chunkCusts)
                    {
                        var custIds = batchCust.Select(c => c.CustID).Distinct();

                        custInfo = _ctsCustomerService.GetMissingCustInfo(custIds);

                        if (custInfo.CustomerInfos.Any())
                        {
                            isSuccess = _ctsCustomerService.InsertNewCustomer(true, custInfo, new InsertNewCustRescanEntity
                            {
                                InternalBatchSize = batchSize,
                                ExternalBatchSize = batchSize,
                                IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                                IsInsertMissingCust = true,
                                InsertVIPURL = appSettings.InsertVIPURL,
                                InsertBAURL = appSettings.InsertBAURL,
                                serviceName = "InsertMissingCustomer"
                            });

                            if (isSuccess)
                            {
                                _ctsCustomerService.CompleteMissingCustomer(string.Join(',', batchCust.Select(c => c.ID).Distinct()));
                            }
                            else
                            {
                                return Task.FromResult(isSuccess);
                            }
                        }
                    }
                }

                if (notMissingCusts.Any())
                {
                    _ctsCustomerService.CompleteMissingCustomer(string.Join(',', notMissingCusts.Select(c => c.ID).Distinct()));
                }

                return Task.FromResult(isSuccess);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateCTSCustomerInfo(int packageSize, int waitTime)
        {
            TranformCustModel updatedCustFromCustInfo;
            TranformCustModel updatedCustFromCustomer;
            TranformCustModel updatedCustFromProductStatus;

            try
            {
                var isWriteLog = appSettings.TransformCTSCustomerIsWriteLog;
                var output = _ctsCustomerService.GetLastUpdatedTime();
                updatedCustFromCustInfo = _ctsCustomerService.GetUpdatedCustFromCustInfoHistory(output.LastUpdateTimeCustInfo, output.LastUpdateCustIDCustInfo, packageSize, waitTime);
                updatedCustFromCustomer = _ctsCustomerService.GetUpdatedCustFromCustomerHistory(output.LastUpdateTimeCustomer, output.LastUpdateCustIDCustomer, packageSize, waitTime);
                updatedCustFromProductStatus = _ctsCustomerService.GetUpdatedCustFromProductStatusHistory(output.LastUpdateTimeCustProductStatus, output.LastUpdateCustIDCustProductStatus, packageSize, waitTime);

                if (_ctsCustomerService.IsValidData(updatedCustFromCustomer)
                    || _ctsCustomerService.IsValidData(updatedCustFromCustInfo)
                    || _ctsCustomerService.IsValidData(updatedCustFromProductStatus))
                {
                    _ctsCustomerService.UpdateCustomer(updatedCustFromCustInfo, UpdateCustomer.FromCustInfo, isWriteLog);
                    _ctsCustomerService.UpdateCustomer(updatedCustFromCustomer, UpdateCustomer.FromCustomer, isWriteLog);
                    _ctsCustomerService.UpdateCustomer(updatedCustFromProductStatus, UpdateCustomer.FromProductStatus, isWriteLog);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }


        #region Update UserName2

        public Task<bool> UpdateUsername2ForCustMain(int batchSize)
        {
            try
            {
                UInt64 lastCTSCustId = _systemParameterServices.GetLastScannedId2(CTS_SystemParameters.CustomerNullUsername2_LastCTSCustID);

                _ctsCustomerService.UpdateUserName2Package(ref lastCTSCustId, false, batchSize);

                _systemParameterServices.UpdateTimingExecution(lastCTSCustId.ToString(), CTS_SystemParameters.CustomerNullUsername2_LastCTSCustID);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);

                return Task.FromResult(false);
            }
        }


        public Task<bool> UpdateUsername2ForCustSub(int batchSize)
        {
            try
            {
                UInt64 lastCTSCustId = 0;
                bool isContinue = false;

                do
                {
                    isContinue = _ctsCustomerService.UpdateUserName2Package(ref lastCTSCustId, true, batchSize);

                } while (isContinue);

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

        #endregion Update UserName2

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
                        var missingCustomers = _ctsCustomerService.CheckMissingCustomer(reactivatedCustomersWithBatch.Select(x => (int)x.CustID));
                        _ctsCustomerService.InsertMissingCustomerLatencyToQueue(missingCustomers);
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

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }
        #endregion

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

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        public Task<bool> InactivateNewCustomers(int batchSize)
        {
            try
            {
                var isContinue = true;

                do
                {
                    var inactivateCusts = _ctsCustomerService.GetInactivateNewCustomers();

                    if (inactivateCusts.CustInfos != null && inactivateCusts.CustInfos.Any())
                    {
                        var custIDs = inactivateCusts.CustInfos.Select(x => x.CustID).Distinct();

                        var rescanConfig = new RescanParametersEntity
                        {
                            IsToQueue = false,
                            FunctionId = FunctionId.InactivateNewCustomer,
                            RescanNormalType = RescanNormalType.Auto_InactiveCustomer,
                            InternalBatchSize = batchSize,
                            ServiceName = "InactiveNewCustomer",
                            PushDataExternal = new PushDataToExternalEntity
                            {
                                RoleGroup = RoleGroupType.Member,
                                IsUpdateDangerToMainDB = true,
                                ExternalBatchSize = batchSize,
                                ServiceName = "InactiveNewCustomer",
                                IsSyncAFCData = appSettings.IsSyncAFCData,
                                AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                                AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                            }
                        };

                        rescanCustClassificationService.RescanCustomerClassification(custIDs, rescanConfig);

                        isContinue = true;
                    }
                    else
                    {
                        isContinue = false;
                    }

                    _systemParameterServices.UpdateTimingExecution(inactivateCusts.LastCategoryID.ToString(), CTS_SystemParameters.InactiveNewMember_LastCategoryID);
                    _systemParameterServices.UpdateTimingExecution(inactivateCusts.LastCustID.ToString(), CTS_SystemParameters.InactiveNewMember_LastCustID);

                    Thread.Sleep(150);

                } while (isContinue);

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

        private bool PushDataToExternal(IEnumerable<long> custIDs, string serviceName)
        {
            return custClassToMainDBService.PushExternalPackageByCustId(custIDs.Distinct(), new PushDataToExternalEntity
            {
                RoleGroup = RoleGroupType.Member,
                ExternalBatchSize = appSettings.InsertReactivatedCustomersBatchSize,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = appSettings.InsertReactivatedCustomersIsWriteLog,
                ServiceName = serviceName
            });
        }

        #endregion

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

            var custInfo = _ctsCustomerService.GetMissingCustInfo(insertNewCusts);

            return _ctsCustomerService.InsertNewCustomer(false, custInfo, new InsertNewCustRescanEntity
            {
                InternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                ExternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                IsInsertMissingCust = false,
                InsertVIPURL = appSettings.InsertVIPURL,
                InsertBAURL = appSettings.InsertBAURL,
                serviceName = "ReactivatedCustomer"
            });
        }
        #endregion Private Methods
    }
}