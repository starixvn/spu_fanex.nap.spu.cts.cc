namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerJobService
    {
        Task<bool> UpdateCTSCustomerInfo(int packageSize, int waitTime);

        Task<bool> UpdateUsername2ForCustMain(int batchSize);

        Task<bool> UpdateUsername2ForCustSub(int batchSize);

        Task<bool> InactivateNewCustomers(int batchSize);
    }

    public class CTSCustomerJobService : ICTSCustomerJobService
    {
        private readonly ICTSCustomerService _ctsCustomerService;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private readonly IInsertNewCustomerService _insertNewCustomerService;
        private readonly IMissingCustomerService _missingCustomerService;
        private readonly IAppSettings appSettings;

        public CTSCustomerJobService(
            ICTSCustomerService ctscustomerService,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IRescanCustClassificationService rescanCustClassificationService,
            IAppSettings appSettings,
            IInsertNewCustomerService insertNewCustomerService,
            IMissingCustomerService missingCustomerService)
        {
            _ctsCustomerService = ctscustomerService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.rescanCustClassificationService = rescanCustClassificationService;
            this._systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
            _insertNewCustomerService = insertNewCustomerService;
            _missingCustomerService = missingCustomerService;
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

                logMessage.LogError();
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

                logMessage.LogError();

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

                logMessage.LogError();

                return Task.FromResult(false);
            }
        }

        #endregion Update UserName2

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

                logMessage.LogError();
                return Task.FromResult(false);
            }
        }
    }
}