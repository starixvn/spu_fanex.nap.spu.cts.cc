namespace Fanex.NAP.SPU.CTS.CC.JobService.Services
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class ProblemAccountJobService : IProblemAccountJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ICustomerEvidenceInsertServices custEvidenceServices;
        private readonly IDailyProblemClassificationServices dailyProblemClassificationServices;
        private readonly IAppSettings appSettings;

        public ProblemAccountJobService(
            ISentryClient sentryClient,
            IProblemAccountServices problemAccountServices,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService pushDataToExternalService,
            IAntiFraudCenterServices antiFraudCenterServices,
            ICustomerEvidenceInsertServices custEvidenceServices,
            IDailyProblemClassificationServices dailyProblemClassificationServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.problemAccountServices = problemAccountServices;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = pushDataToExternalService;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.custEvidenceServices = custEvidenceServices;
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;
            this.appSettings = appSettings;
            this.serviceName = "ProblemAccountInsert";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            bool response = true;

            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    response = JobRun(batchExternalSize, batchInternalSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (batchExternalSize, batchInternalSize), new NullReferenceException(), sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(response);
        }

        #region Private Methods

        private bool JobRun(int batchExternalSize, int batchInternalSize)
        {
            var lastScannedTime = systemParameterServices.GetLastScannedDateTime(SystemParameters.ProblemAccountLastScannedTime);

            var savedDataTime = DateTime.Now.AddSeconds(-10);

            var problemAccounts = problemAccountServices.GetProblemAccounts(lastScannedTime, serviceName, appSettings.ProblemAccountIsWriteLog);

            var isSuccess = InsertProblemAccount(problemAccounts, batchExternalSize, batchInternalSize);

            if (isSuccess)
            {
                systemParameterServices.UpdateTimingExecution(savedDataTime.ToString(Constants.CONVERT_TO_SQL_DATETIME), SystemParameters.ProblemAccountLastScannedTime);
            }

            return isSuccess;
        }

        private bool InsertProblemAccount(IEnumerable<ProblemAccountsEntity> problemAccounts, int batchExternalSize, int batchInternalSize)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                custClassToMainDBService.LogInputData(new LogInputViewEntity { ProblemAccounts = problemAccounts }, FunctionId.InsertPAJob, serviceName);

                var problemAccountsChunk = problemAccounts.Distinct().Chunk(batchInternalSize);

                foreach (var probAccountBatch in problemAccountsChunk)
                {
                    var probAccountLosing = dailyProblemClassificationServices.GetProblemAccountLosingClassification(probAccountBatch.Select(c => c.CustID), FunctionId.InsertPAJob);

                    var result = problemAccountServices.InsertProblemAccount(probAccountBatch, probAccountLosing, false, serviceName);

                    var isSuccess = (result != null && result.ErrorCode == 0);

                    if (isSuccess && result.CustIds != null && result.CustIds.Any())
                    {
                        InsertEvidenceFromNapPA(probAccountBatch);

                        isSuccess = PushDataToExternal(result.CustIds, batchExternalSize);
                    }

                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void InsertEvidenceFromNapPA(IEnumerable<ProblemAccountsEntity> problemAccounts)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var custEvidences = custEvidenceServices.GetCustEvidenceByReasonId(problemAccounts);

                if (custEvidences != null && custEvidences.Any())
                {
                    custEvidenceServices.InsertCustEvidenceFromProblemAccount(custEvidences);
                }
            }
        }

        private bool PushDataToExternal(IEnumerable<long> custIds, int batchExternalSize)
        {
            var isSuccess = true;

            if (custIds != null && custIds.Any())
            {
                Parallel.Invoke(() =>
                {
                    isSuccess = custClassToMainDBService.PushDataToExternal(custIds, new PushDataToExternalEntity
                    {
                        ExternalBatchSize = batchExternalSize,
                        FeedSoon88API = appSettings.Soon88FeedDataAPI,
                        IsPushCCToSoon88 = true,
                        IsUpdateDangerToMainDB = true,
                        DangerLevelType = DangerLevel.ABI,
                        IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                        ServiceName = serviceName
                    });
                },
                () =>
                {
                    custEvidenceServices.UpdateBetLimitOfFlaggedCustomer(custIds);
                },
                () =>
                {
                    antiFraudCenterServices.InsertAFCAccounts(custIds, FromFlow.CTSServices, appSettings.IsSyncAFCData, appSettings.AFCInsertAccountURL);
                });
            }

            return isSuccess;
        }

        #endregion Private Methods
    }
}