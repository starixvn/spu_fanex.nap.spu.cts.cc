namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustClassToMainDBService
    {
        bool PushExternalProblemAcountPackage(IEnumerable<long> custIds, PushDataToExternalEntity parameters);

        IEnumerable<LatestCCDangerLevelEntity> GetLatestCCAndDangerLevel(IEnumerable<long> custIds);

        bool PushCustClassificationToMainDB(IEnumerable<LatestCCDangerLevelEntity> lastestClassifications, string serviceName);

        bool RetryPushCCToMainDB(IEnumerable<LatestClassificationEntity> customerClassifications, int retryTime, string serviceName);

        bool UpdateDangerLevelToMainDB(IEnumerable<LatestCCDangerLevelEntity> lastestClassifications, string serviceName);

        bool RetryUpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevels, int dangerTypeId, int retryTime, string serviceName);

        bool PushDataToExternal(IEnumerable<long> custIds, PushDataToExternalEntity paras);

        void LogInputData(LogInputViewEntity content, FunctionId funcId, string serviceName);
    }

    public class CustClassToMainDBService : ICustClassToMainDBService
    {
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly IFeedSoon88Services feedSoon88Services;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISentryClient sentryClient;

        public CustClassToMainDBService(
            ICustClassToMainDBDataAccess custClassToMainDBDataAccess,
            ISentryClient sentryClient,
            IFeedSoon88Services feedSoon88Services,
            IAntiFraudCenterServices antiFraudCenterServices)
        {
            this.custClassToMainDBDataAccess = custClassToMainDBDataAccess;
            this.feedSoon88Services = feedSoon88Services;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.sentryClient = sentryClient;
        }

        public bool PushExternalProblemAcountPackage(IEnumerable<long> custIds, PushDataToExternalEntity parameters)
        {
            var isSuccess = true;

            if (custIds != null && custIds.Any())
            {
                isSuccess = PushDataToExternal(custIds.Distinct(), parameters);

                antiFraudCenterServices.InsertAFCAccounts(custIds.Distinct(), FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCInsertAccountURL);
            }

            return isSuccess;
        }

        public bool PushDataToExternal(IEnumerable<long> custIds, PushDataToExternalEntity parameters)
        {
            if (custIds == null || !custIds.Any())
            {
                return true;
            }

            var custIdsChunk = custIds.Distinct().Chunk(parameters.ExternalBatchSize);

            foreach (var custids in custIdsChunk)
            {
                var lastestClassifications = GetLatestCCAndDangerLevel(custids);

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    bool isSuccess = IsPushExternalSuccess(lastestClassifications, parameters);

                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsPushExternalSuccess(IEnumerable<LatestCCDangerLevelEntity> lastestClassifications, PushDataToExternalEntity paras)
        {
            bool isSuccess = true;

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                isSuccess = PushCustClassificationToMainDB(lastestClassifications, paras.ServiceName);

                if (paras.IsPushCCToSoon88)
                {
                    feedSoon88Services.FeedCCToSoon88(lastestClassifications, paras.FeedSoon88API, paras.ServiceName);
                }

                if (paras.IsUpdateDangerToMainDB && isSuccess)
                {
                    isSuccess = UpdateDangerLevelToMainDB(lastestClassifications, paras.ServiceName);
                }
            }

            return isSuccess;
        }

        public bool PushCustClassificationToMainDB(IEnumerable<LatestCCDangerLevelEntity> lastestClassifications, string serviceName)
        {
            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var custClassification = lastestClassifications.Select(c => new LatestClassificationEntity()
                {
                    CustId = c.CustID,
                    Category = c.CategoryID,
                    CustomerClass = c.CustomerClass
                });

                return RetryPushCCToMainDB(custClassification, Constants.RetryPushCCToExternal, serviceName);
            }

            return true;
        }

        public IEnumerable<LatestCCDangerLevelEntity> GetLatestCCAndDangerLevel(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                return custClassToMainDBDataAccess.GetLatestCCAndDangerLevel(custIds.Distinct());
            }

            return Enumerable.Empty<LatestCCDangerLevelEntity>();
        }

        public bool UpdateDangerLevelToMainDB(IEnumerable<LatestCCDangerLevelEntity> lastestClassifications, string serviceName)
        {
            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var groupDangerType = lastestClassifications.GroupBy(c => c.DangerLevelType);

                if (groupDangerType != null && groupDangerType.Any())
                {
                    foreach (var dangerBatch in groupDangerType)
                    {
                        var updateDangerLevel = dangerBatch.Select(c => new DangerLevelEntity()
                        {
                            CustId = c.CustID,
                            DangerLevel = c.DangerLevel
                        });

                        var isUpdateSuccessful = RetryUpdateDangerLevelToMainDB(updateDangerLevel, dangerBatch.Key, Constants.RetryPushCCToExternal, serviceName);

                        if (!isUpdateSuccessful)
                        {
                            return isUpdateSuccessful;
                        }
                    }
                }
            }

            return true;
        }

        public bool RetryPushCCToMainDB(IEnumerable<LatestClassificationEntity> customerClassifications, int retryTime, string serviceName)
        {
            if (customerClassifications == null || !customerClassifications.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    var result = custClassToMainDBDataAccess.UpdateCustomerGeneralClassificationtoMainDB(customerClassifications);

                    if (result.ErrorCode != 0 && retryTime == 0)
                    {
                        Utilities.LogSentryError(serviceName, "PushCustomerClassificationToMain fail", customerClassifications, new Exception(result.ErrorMsg), sentryClient);
                        return false;
                    }
                    else if (result.ErrorCode != 0)
                    {
                        retryTime--;
                    }
                    else
                    {
                        retryTime = -1;
                    }
                }
                catch (Exception ex)
                {
                    if (retryTime == 0)
                    {
                        Utilities.LogSentryError(serviceName, "PushCustomerClassificationToMain throw exception", customerClassifications, ex, sentryClient);
                        return false;
                    }

                    retryTime--;
                }
            }

            return true;
        }

        public bool RetryUpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevels, int dangerTypeId, int retryTime, string serviceName)
        {
            if (dangerLevels == null || !dangerLevels.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    var result = custClassToMainDBDataAccess.UpdateDangerLevelToMainDB(dangerLevels, dangerTypeId);

                    if (result.ErrorCode != 0 && retryTime == 0)
                    {
                        Utilities.LogSentryError(serviceName, "UpdateDangerLevelToMain fail", (dangerLevels, dangerTypeId), new Exception(result.ErrorMsg), sentryClient);
                        return false;
                    }
                    else if (result.ErrorCode != 0)
                    {
                        retryTime--;
                    }
                    else
                    {
                        retryTime = -1;
                    }
                }
                catch (Exception ex)
                {
                    if (retryTime == 0)
                    {
                        Utilities.LogSentryError(serviceName, "UpdateDangerLevelToMain thrown exception", (dangerLevels, dangerTypeId), ex, sentryClient);
                        return false;
                    }

                    retryTime--;
                }
            }

            return true;
        }

        public void LogInputData(LogInputViewEntity content, FunctionId funcId, string serviceName)
        {
            try
            {
                var inputData = MappingLogInputData(content);

                if (inputData != null && inputData.Any())
                {
                    var result = custClassToMainDBDataAccess.LogInputData(inputData, funcId);

                    if (!string.IsNullOrEmpty(result.ErrorMsg))
                    {
                        Utilities.LogSentryError(serviceName, "LogInputData fail", (content, funcId), new Exception(result.ErrorMsg), sentryClient);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "LogInputData fail", (content, funcId), ex, sentryClient);
            }
        }

        private IEnumerable<LogInputDataEntity> MappingLogInputData(LogInputViewEntity content)
        {
            var logInputData = new List<LogInputDataEntity>();

            if (content != null && content.RobotUser != null && content.RobotUser.Any())
            {
                content.RobotUser.ToList().ForEach(c =>
                {
                    logInputData.Add(new LogInputDataEntity
                    {
                        CustId = c.CustID,
                        InputValue = new { c.RobotCounter, c.CategoryID, c.CreatedBy }.ToString()
                    });
                });
            }

            return logInputData;
        }
    }
}