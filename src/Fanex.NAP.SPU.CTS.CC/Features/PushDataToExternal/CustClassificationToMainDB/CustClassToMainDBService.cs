namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustClassToMainDBService
    {
        #region General

        bool PushExternalPackageByCustId(IEnumerable<long> custIds, PushDataToExternalEntity paras);

        bool PushExternalProblemAcountPackage(IEnumerable<long> custIds, PushDataToExternalEntity parameters);

        bool PushCustomerClassification(IEnumerable<LatestClassificationEntity> customerClassifications, int retryTime, string serviceName);

        #endregion

        #region BySport
        bool PushCCBySportToMainDB(IEnumerable<InsertNormalAccountCustInfoEntity> custInfos, string serviceName, int batchExternalSize);

        bool RetryPushCCBySportToMainDB(IEnumerable<GetCustomerClassificationBySportEntity> classificationBySports, int retryTime, string serviceName);

        #endregion
    }

    public class CustClassToMainDBService : ICustClassToMainDBService
    {
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISentryClient sentryClient;

        public CustClassToMainDBService(
            ICustClassToMainDBDataAccess custClassToMainDBDataAccess,
            ISentryClient sentryClient,
            IAntiFraudCenterServices antiFraudCenterServices)
        {
            this.custClassToMainDBDataAccess = custClassToMainDBDataAccess;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.sentryClient = sentryClient;
        }

        #region Public methods - General

        public bool PushExternalProblemAcountPackage(IEnumerable<long> custIds, PushDataToExternalEntity parameters)
        {
            var isSuccess = true;

            if (custIds != null && custIds.Any())
            {
                isSuccess = PushExternalPackageByCustId(custIds.Distinct(), parameters);

                antiFraudCenterServices.InsertAFCAccounts(custIds.Distinct(), FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCInsertAccountURL);
            }

            return isSuccess;
        }

        public bool PushExternalPackageByCustId(IEnumerable<long> custIds, PushDataToExternalEntity parameters)
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

        public bool PushCustomerClassification(IEnumerable<LatestClassificationEntity> customerClassifications, int retryTime, string serviceName)
        {
            if (customerClassifications == null || !customerClassifications.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    var result = custClassToMainDBDataAccess.UpdateCustomerClassification(customerClassifications);

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

        #endregion

        #region Public methods - BySport

        public bool PushCCBySportToMainDB(IEnumerable<InsertNormalAccountCustInfoEntity> custInfos, string serviceName, int batchExternalSize)
        {
            var custInfoChunks = custInfos.Chunk(batchExternalSize).ToList();

            foreach (var custs in custInfoChunks)
            {
                var ccBySports = this.GetCustomerClassificationBySports(custs);

                var isSuccess = this.RetryPushCCBySportToMainDB(ccBySports, Constants.InternalRetry, serviceName);

                if (!isSuccess)
                {
                    return false;
                }
            }

            return true;
        }

        public bool RetryPushCCBySportToMainDB(IEnumerable<GetCustomerClassificationBySportEntity> classificationBySports, int retryTime, string serviceName)
        {
            if (classificationBySports != null && classificationBySports.Any())
            {
                var custClassification = classificationBySports.Select(x => new CustomerClassificationUpdateBySportToMainDBEntity
                {
                    CustID = x.CustID,
                    SportID = x.SportID,
                    CustomerClass = x.CustomerClass
                });

                while (retryTime >= 0)
                {
                    try
                    {
                        var result = custClassToMainDBDataAccess.UpdateCustomerClassificationBySport(custClassification);

                        if (result.ErrorCode != 0 && retryTime == 0)
                        {
                            Utilities.LogSentryError(serviceName, "UpdateCustomerClassificationBySportToMainDB failed", custClassification, new Exception(result.ErrorMsg), sentryClient);
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
                            Utilities.LogSentryError(serviceName, "UpdateCustomerClassificationBySportToMainDB thrown exception", custClassification, ex, sentryClient);
                            return false;
                        }

                        retryTime--;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Private methods

        private bool PushCustClassificationToMainDB(IEnumerable<LatestCCDangerLevelEntity> lastestClassifications, string serviceName)
        {
            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var custClassification = lastestClassifications.Select(c => new LatestClassificationEntity()
                {
                    CustId = c.CustID,
                    Category = c.CategoryID,
                    CustomerClass = c.CustomerClass
                });

                return PushCustomerClassification(custClassification, Constants.RetryPushCCToExternal, serviceName);
            }

            return true;
        }

        private bool IsPushExternalSuccess(IEnumerable<LatestCCDangerLevelEntity> lastestClassifications, PushDataToExternalEntity paras)
        {
            bool isSuccess = true;

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                isSuccess = PushCustClassificationToMainDB(lastestClassifications, paras.ServiceName);

                if (paras.IsUpdateDangerToMainDB && isSuccess)
                {
                    isSuccess = UpdateDangerLevelToMainDB(lastestClassifications, paras.ServiceName);
                }
            }

            return isSuccess;
        }

        private IEnumerable<LatestCCDangerLevelEntity> GetLatestCCAndDangerLevel(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                return custClassToMainDBDataAccess.GetLatestCCAndDangerLevel(custIds.Distinct());
            }

            return Enumerable.Empty<LatestCCDangerLevelEntity>();
        }

        private bool UpdateDangerLevelToMainDB(IEnumerable<LatestCCDangerLevelEntity> lastestClassifications, string serviceName)
        {
            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var groupDangerType = lastestClassifications.Where(c => c.DangerLevel != null).GroupBy(c => c.DangerLevelType);

                if (groupDangerType != null && groupDangerType.Any())
                {
                    foreach (var dangerBatch in groupDangerType)
                    {
                        var updateDangerLevel = dangerBatch.Select(c => new DangerLevelEntity()
                        {
                            CustId = c.CustID,
                            DangerLevel = c.DangerLevel.Value
                        });

                        var isUpdateSuccessful = PushDangerLevel(updateDangerLevel, dangerBatch.Key.Value, Constants.RetryPushCCToExternal, serviceName);

                        if (!isUpdateSuccessful)
                        {
                            return isUpdateSuccessful;
                        }
                    }
                }
            }

            return true;
        }

        private bool PushDangerLevel(IEnumerable<DangerLevelEntity> dangerLevels, int dangerTypeId, int retryTime, string serviceName)
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

        private IEnumerable<GetCustomerClassificationBySportEntity> GetCustomerClassificationBySports(IEnumerable<InsertNormalAccountCustInfoEntity> custInfos)
        {
            if (custInfos != null && custInfos.Any())
            {
                return custClassToMainDBDataAccess.GetCustomerClassificationBySports(custInfos.Distinct());
            }

            return Enumerable.Empty<GetCustomerClassificationBySportEntity>();
        }

        #endregion
    }
}