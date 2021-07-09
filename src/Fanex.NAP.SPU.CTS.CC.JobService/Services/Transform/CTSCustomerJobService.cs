namespace Fanex.NAP.SPU.CTS.JobService.Services.Transform
{
    using Fanex.NAP.SPU.CTS.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Features.UserPermission;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerJobService
    {
        Task<bool> InsertNewCustomer();
    }

    public class CTSCustomerJobService : ICTSCustomerJobService
    {
        private readonly ICTSCustomerService ctscustomerService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public CTSCustomerJobService(ISentryClient sentryClient, ICTSCustomerService ctscustomerService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.ctscustomerService = ctscustomerService;
            this.appSettings = appSettings;
        }

        public Task<bool> InsertNewCustomer()
        {
            try
            {
                var isWriteLog = appSettings.TransformCTSCustomerIsWriteLog;
                var soon88Url = appSettings.Soon88FeedDataAPI;
                var batchSize = appSettings.TransformCTSCustomerBatchSize;

                var lastScanned = ctscustomerService.GetLastInsertCust();
                var customers = ctscustomerService.GetCustomerFromMainDBForInsertNew(batchSize, lastScanned.LastCustID);
                var custSubs = ctscustomerService.GetCustSubFromMainDB(batchSize, lastScanned.LastCustSubID).ToList();
                var custsToCTS = ctscustomerService.PrepareData(customers, custSubs);

                var result = ctscustomerService.InsertNewCustomer(custsToCTS, isWriteLog, batchSize, soon88Url);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateCTSCustomerInfo(int packageSize)
        {
            TranformCustModel updatedCustFromCustInfo;
            TranformCustModel updatedCustFromCustomer;

            try
            {
                var isWriteLog = appSettings.TransformCTSCustomerIsWriteLog;
                var output = ctscustomerService.GetLastUpdatedTime();
                updatedCustFromCustInfo = ctscustomerService.GetUpdatedCustFromCustInfoHistory(output.LastUpdateTimeCustInfo, output.LastUpdateCustIDCustInfo, packageSize);
                updatedCustFromCustomer = ctscustomerService.GetUpdatedCustFromCustomerHistory(output.LastUpdateTimeCustomer, output.LastUpdateCustIDCustomer, packageSize);

                if (ctscustomerService.IsValidData(updatedCustFromCustInfo, updatedCustFromCustomer))
                {
                    ctscustomerService.UpdateCustomer(updatedCustFromCustInfo, (int)UpdateCustomer.FromCustInfo, isWriteLog);
                    ctscustomerService.UpdateCustomer(updatedCustFromCustomer, (int)UpdateCustomer.FromCustomer, isWriteLog);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateUsername2(int batchSize)
        {
            try
            {
                long preCTSCustId = 0;
                var custsNullUsername2 = ctscustomerService.GetCustWithNullUserName2(batchSize, preCTSCustId);
                if (custsNullUsername2 is not null && custsNullUsername2.Any())
                {
                    var custFromMainDB = ctscustomerService.GetCustWithHavingUserName2(custsNullUsername2);
                    preCTSCustId = custsNullUsername2.Max(c => c.CTSCustID);
                    ctscustomerService.UpdateUsername2(custFromMainDB);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return Task.FromResult(false);
            }
            
        }
    }
}
