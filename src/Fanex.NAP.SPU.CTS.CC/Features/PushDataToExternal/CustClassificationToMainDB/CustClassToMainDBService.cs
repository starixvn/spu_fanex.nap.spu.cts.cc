namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustClassToMainDBService
    {
        IEnumerable<LatestClassificationEntity> GetLatestClassification(IEnumerable<long> custIds, string serviceName, bool isWriteLog);

        bool PushCustClassificationToMainDB(IEnumerable<LatestClassificationEntity> lastestClassifications, string serviceName, bool isWriteLog);

        bool RetryPushCCToMainDB(IEnumerable<LatestClassificationEntity> customerClassifications, int retryTime, string serviceName);

        bool UpdateAppliedCustClassification(IEnumerable<LatestClassificationEntity> lastestClassifications, string serviceName);

        bool UpdateDangerLevelToMainDB(IEnumerable<LatestClassificationEntity> lastestClassifications, DangerLevel dangerLevelType, string serviceName, bool isWriteLog);

        bool RetryUpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevels, DangerLevel typeId, int retryTime, string serviceName);

        bool PushDataToExternal(IEnumerable<long> custIds, PushDataToExternalEntity paras);

        void LogInputData(LogInputViewEntity content, FunctionId funcId, string serviceName);
    }

    public class CustClassToMainDBService : ICustClassToMainDBService
    {
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly IFeedSoon88Services feedSoon88Services;
        private readonly ISentryClient sentryClient;

        public CustClassToMainDBService(
            ICustClassToMainDBDataAccess custClassToMainDBDataAccess,
            ISentryClient sentryClient,
            IFeedSoon88Services feedSoon88Services)
        {
            this.custClassToMainDBDataAccess = custClassToMainDBDataAccess;
            this.feedSoon88Services = feedSoon88Services;
            this.sentryClient = sentryClient;
        }

        public bool PushDataToExternal(IEnumerable<long> custIds, PushDataToExternalEntity paras)
        {
            if (custIds == null || !custIds.Any())
            {
                return true;
            }

            var custIdsChunk = custIds.Distinct().Chunk(paras.ExternalBatchSize);

            foreach (var custids in custIdsChunk)
            {
                var lastestClassifications = GetLatestClassification(custids, paras.ServiceName, paras.IsWriteLog);

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    bool isSuccess = IsPushExternalSuccess(lastestClassifications, paras);

                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsPushExternalSuccess(IEnumerable<LatestClassificationEntity> lastestClassifications, PushDataToExternalEntity paras)
        {
            bool isSuccess = true;

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                isSuccess = PushCustClassificationToMainDB(lastestClassifications, paras.ServiceName, paras.IsWriteLog);

                if (paras.IsPushCCToSoon88)
                {
                    feedSoon88Services.FeedCCToSoon88(lastestClassifications, paras.FeedSoon88API, paras.ServiceName, paras.IsWriteLog);
                }

                if (paras.IsUpdateDangerToMainDB && isSuccess)
                {
                    isSuccess = UpdateDangerLevelToMainDB(lastestClassifications, paras.DangerLevelType, paras.ServiceName, paras.IsWriteLog);
                }
            }

            return isSuccess;
        }

        public bool PushCustClassificationToMainDB(IEnumerable<LatestClassificationEntity> lastestClassifications, string serviceName, bool isWriteLog)
        {
            bool isSuccess = true;

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                isSuccess = RetryPushCCToMainDB(lastestClassifications, Constants.RetryPushCCToExternal, serviceName);

                if (isSuccess)
                {
                    UpdateAppliedCustClassification(lastestClassifications, serviceName);
                }
            }

            Utilities.LogSentryInfo(serviceName, "PushCCToMainDB done", (isSuccess, lastestClassifications), isWriteLog, sentryClient);
            return isSuccess;
        }

        public IEnumerable<LatestClassificationEntity> GetLatestClassification(IEnumerable<long> custIds, string serviceName, bool isWriteLog)
        {
            var lastestClassification = new List<LatestClassificationEntity>();

            if (custIds != null && custIds.Any())
            {
                string custIdsToString = string.Join(",", custIds);

                lastestClassification = custClassToMainDBDataAccess.GetLatestClassification(custIdsToString)?.ToList();
            }

            Utilities.LogSentryInfo(serviceName, "Get lastestClassifications", lastestClassification, isWriteLog, sentryClient);
            return lastestClassification;
        }

        public bool UpdateAppliedCustClassification(IEnumerable<LatestClassificationEntity> lastestClassifications, string serviceName)
        {
            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var jsonData = JsonConvert.SerializeObject(lastestClassifications);

                var result = custClassToMainDBDataAccess.UpdateAppliedCustomerClassification(jsonData);

                if (result.ErrorCode == -1)
                {
                    Utilities.LogSentryError(serviceName, "Update Apllied CC throw exception", lastestClassifications, new Exception(result.ErrorMsg), sentryClient);

                    return false;
                }
            }

            return true;
        }

        public bool UpdateDangerLevelToMainDB(IEnumerable<LatestClassificationEntity> lastestClassifications, DangerLevel dangerLevelType, string serviceName, bool isWriteLog)
        {
            if (lastestClassifications == null || !lastestClassifications.Any())
            {
                return true;
            }

            var dangerLevels = custClassToMainDBDataAccess.GetDangerLevel(string.Join(",", lastestClassifications.Select(c => c.CustId)), dangerLevelType);
            Utilities.LogSentryInfo(serviceName, "Get GetDangerLevel", dangerLevels, isWriteLog, sentryClient);

            if (dangerLevels != null && dangerLevels.Any())
            {
                var isUpdateSuccessful = RetryUpdateDangerLevelToMainDB(dangerLevels, dangerLevelType, Constants.RetryPushCCToExternal, serviceName);

                if (isUpdateSuccessful)
                {
                    var jsonUpdateApplyCCs = JsonConvert.SerializeObject(dangerLevels);

                    custClassToMainDBDataAccess.UpdateAppliedDangerLevel(jsonUpdateApplyCCs, dangerLevelType);
                }

                Utilities.LogSentryInfo(serviceName, "UpdateDangerLevelToMainDB done", (isUpdateSuccessful, lastestClassifications), isWriteLog, sentryClient);

                return isUpdateSuccessful;
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

        public bool RetryUpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevels, DangerLevel typeId, int retryTime, string serviceName)
        {
            if (dangerLevels == null || !dangerLevels.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    var result = custClassToMainDBDataAccess.UpdateDangerLevelToMainDB(dangerLevels, typeId);

                    if (result.ErrorCode != 0 && retryTime == 0)
                    {
                        Utilities.LogSentryError(serviceName, "UpdateDangerLevelToMain fail", (dangerLevels, typeId), new Exception(result.ErrorMsg), sentryClient);
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
                        Utilities.LogSentryError(serviceName, "UpdateDangerLevelToMain thrown exception", (dangerLevels, typeId), ex, sentryClient);
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
                        CustId = c.CustId,
                        InputValue = c.Counter
                    });
                });
            }

            if (content != null && content.ProblemAccounts != null && content.ProblemAccounts.Any())
            {
                content.ProblemAccounts.ToList().ForEach(c =>
                {
                    logInputData.Add(new LogInputDataEntity
                    {
                        CustId = c.CustID,
                        InputValue = c.ReasonID
                    });
                });
            }

            return logInputData;
        }
    }
}