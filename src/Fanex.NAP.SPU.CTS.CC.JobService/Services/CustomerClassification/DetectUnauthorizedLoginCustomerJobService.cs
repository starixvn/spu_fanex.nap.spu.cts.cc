namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDetectUnauthorizedLoginCustomerJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize, bool IsLicensee);
    }

    public class DetectUnauthorizedLoginCustomerJobService : IDetectUnauthorizedLoginCustomerJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IDetectUnauthorizedLoginCustomerService detectUnauthorizedLoginCustService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        public DetectUnauthorizedLoginCustomerJobService(ISentryClient sentryClient,
            IAppSettings appSettings,
            IDetectUnauthorizedLoginCustomerService detectUnauthorizedLoginCustService, 
            IProblemAccountServices problemAccountServices,
            ICustClassToMainDBService custClassToMainDBService)
        {
            this.serviceName = "DetectUnauthorizedLoginCustomer";
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.detectUnauthorizedLoginCustService = detectUnauthorizedLoginCustService;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize, bool isLicensee)
        {
            try
            {
                var isContinue = true;
                var param = new DetectUnauthorizedLoginCustParam(isLicensee);

                do
                {
                    isContinue = ProcessDetectUnauthorizedLoginCust(batchExternalSize, batchInternalSize, param);
                  
                } while (isContinue);
                
                detectUnauthorizedLoginCustService.UpdateLastScannedID(param.LastScanCustID, 0.ToString());
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool ProcessDetectUnauthorizedLoginCust(int batchExternalSize, int batchInternalSize, DetectUnauthorizedLoginCustParam param)
        {
            var processDate = detectUnauthorizedLoginCustService.GetProcessDate(param.LastScanDate);

            if (processDate.Date > DateTime.Now)
                return false;

            var custFirstBetDates = detectUnauthorizedLoginCustService.GetCustWithFirstBetDate(param, processDate);
            
            if (custFirstBetDates.Any())
            {
                custFirstBetDates.Chunk(batchInternalSize).AsParallel().WithDegreeOfParallelism(4).ForAll(cust =>
                {
                    var unauthorizedCustResult = DetectUnauthorizedLoginCust(cust);
                    InsertProblemAccount(unauthorizedCustResult, batchExternalSize);
                });            

                detectUnauthorizedLoginCustService.UpdateLastScannedID(param.LastScanCustID, custFirstBetDates.Max(x => x.custID).ToString());

                return true;
            }
            else
            {                              
                detectUnauthorizedLoginCustService.UpdateLastScannedID(param.LastScanDate, processDate.ToString(CONVERT_TO_SQL_DATE));

                return true;
            }
        }

        private IEnumerable<DetectUnauthorizedLoginCustEntity> DetectUnauthorizedLoginCust(IEnumerable<CustFirstBetDateEntity> custFirstBetDates)
        {            
            if (custFirstBetDates.Any())
            {
                // Test
                //custFirstBetDates = custFirstBetDates.Where(x => x.custID == 111120).Select(x => new CustFirstBetDateEntity { custID = 1284, FirstbetDate = x.FirstbetDate });
                // End Test

                var custJson = JsonConvert.SerializeObject(custFirstBetDates.Select(x => new CustFirstBetDateModel { CustID = x.custID, FirstBetDate = x.FirstbetDate }));

                return detectUnauthorizedLoginCustService.DetectUnauthorizedLogin(custJson);
            }

            return Enumerable.Empty<DetectUnauthorizedLoginCustEntity>();
        }

        private bool InsertProblemAccount(IEnumerable<DetectUnauthorizedLoginCustEntity> unauthorizedCustResult, int batchExternalSize)
        {
            if (unauthorizedCustResult.Any())
            {
                var pas = unauthorizedCustResult.Select(x =>
                {
                    return new InsertProblemAccountEntity
                    {
                        CustID = x.CustID,
                        CTSCustID = x.CTSCustID,
                        RoleID = x.RoleID,
                        SubscriberID = x.SubscriberID,
                        CategoryID = x.CategoryID,
                        CategoryGroup = x.CategoryID,
                        IsLicensee = x.IsLicensee,
                        IsDirectPA = true,
                        IsFromCTS = true,
                        IsFromTVS = false,
                        IsFromTW = false,
                        CreatedBy = Constants.StarixITId,
                        Remark = JsonConvert.SerializeObject(new RemarkDetectUnauthorizedModel(x))
                    };
                });

                var paCustIDs = problemAccountServices.InsertProblemAccountPackage(pas, FunctionId.DetectUnauthorizedLoginCust, InsertPA_ByType.Category, false, serviceName);

                var paras = new PushDataToExternalEntity
                {
                    ExternalBatchSize = batchExternalSize,
                    FeedSoon88API = appSettings.Soon88FeedDataAPI,
                    IsPushCCToSoon88 = true,
                    IsUpdateDangerToMainDB = true,
                    IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                    ServiceName = serviceName
                };

                custClassToMainDBService.PushDataToExternal(paCustIDs, paras);
            }                                 

            return true;
        }
    }
}
