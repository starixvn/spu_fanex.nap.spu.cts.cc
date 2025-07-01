namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using System.Linq;
    using System;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using System.Collections.Generic;

    public interface ICTSNewCustomerJobService
    {
        Task<bool> InsertNewCustomer(int batchSize);
    }

    public class CTSNewCustomerJobService : ICTSNewCustomerJobService
    {
        private readonly IInsertNewCustomerService _insertNewCustomerService;
        private readonly IMissingCustomerService _missingLatencyCustomerService;
        private readonly IAppSettings _appSettings;

        public CTSNewCustomerJobService(
            IInsertNewCustomerService insertNewCustomerService,
            IMissingCustomerService missingLatencyCustomerService,
            IAppSettings appSettings)
        {
            _insertNewCustomerService = insertNewCustomerService;
            _missingLatencyCustomerService = missingLatencyCustomerService;
            _appSettings = appSettings;
        }

        public Task<bool> InsertNewCustomer(int batchSize)
        {
            try
            {
                var isSuccess = true;
                var customers = new InsertCustomerModel();
                var newSubCustomers = Enumerable.Empty<CustomerMainDBEntity>();

                var lastScanned = _insertNewCustomerService.GetLastInsertCust();


                Parallel.Invoke(
                    () => { customers = GetNewCustomerFromMainDBAndHandleMissingCustomer(batchSize, lastScanned.LastCustID); },
                    () => { newSubCustomers = _insertNewCustomerService.GetCustSubFromMainDBByLastCustSub(batchSize, lastScanned.LastCustSubID); });

                if (customers.CustomerInfos.IsNullOrEmpty() && newSubCustomers.IsNullOrEmpty())
                {
                    _insertNewCustomerService.UpdateComleteAndRetranCustomer(
                    custIdList: new List<InsertNewCustomerResultModel>(),
                    lastCustID: customers.LastInsertCustID,
                    isInsertMissingCust: false);
                }
                else
                {
                    customers.CustomerInfos.AddRange(newSubCustomers);

                    isSuccess = _insertNewCustomerService.InsertNewCustomer(true, customers, new InsertNewCustRescanEntity
                    {
                        InternalBatchSize = batchSize,
                        ExternalBatchSize = batchSize,
                        IsWriteLog = _appSettings.TransformCTSCustomerIsWriteLog,
                        IsInsertMissingCust = false,
                        InsertVIPURL = _appSettings.InsertVIPURL,
                        InsertBAURL = _appSettings.InsertBAURL,
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

                logMessage.LogError();
                return Task.FromResult(false);
            }
        }

        private InsertCustomerModel GetNewCustomerFromMainDBAndHandleMissingCustomer(int numOfAccount, Int64 lastScannedCustID)
        {
            var customers = _insertNewCustomerService.GetCustomerFromMainDBForInsertNew(numOfAccount, lastScannedCustID);

            _missingLatencyCustomerService.InsertMissingCustIdByLatencyToQueue(customers.MissingCustIds, JobMonitor_IssueType.NewCustomer_IncorrectCustInfo);

            return customers;
        }
    }
}
