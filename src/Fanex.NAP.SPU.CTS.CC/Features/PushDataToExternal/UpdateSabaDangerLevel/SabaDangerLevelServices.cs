namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ISabaDangerLevelServices
    {
        ISabaDangerLevelServices Start();

        bool GetSabaDangerLevelForScanning();

        ISabaDangerLevelServices UpdateSabaDangerLevelToMainDB(ISentryClient sentryClient, string serviceName, int batchSize);

        ISabaDangerLevelServices UpdateLogSabaDangerLevel();

        bool UpdateLastScan();

        #region For Initialzing Data

        ISabaDangerLevelServices StartForInitialzingData();

        bool GetSabaDangerLevelForInitialization();

        ISabaDangerLevelServices UpdateSabaDangerLevelToMainDBForInitialization(ISentryClient sentryClient, string serviceName, int batchSize);

        ISabaDangerLevelServices UpdateLogSabaDangerLevelForInitialization();

        bool UpdateLastScanForInitialization();

        #endregion For Initialzing Data
    }

    public class SabaDangerLevelServices : ISabaDangerLevelServices
    {
        private readonly ISabaDangerLevelDataAccess sabaDangerLevelDataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private SabaDangerLevelResultEntity sabaDangerLevel;
        private SabaDangerLevelInitResultEntity initSabaDangerLevel;

        public SabaDangerLevelServices(ISabaDangerLevelDataAccess sabaDangerLevelDataAccess, ISystemParameterServices systemParameterServices)
        {
            this.sabaDangerLevelDataAccess = sabaDangerLevelDataAccess;
            this.systemParameterServices = systemParameterServices;
        }

        public ISabaDangerLevelServices Start()
        {
            sabaDangerLevel = new SabaDangerLevelResultEntity();

            return this;
        }

        public bool GetSabaDangerLevelForScanning()
        {
            sabaDangerLevel = sabaDangerLevelDataAccess.GetSabaDangerLevelForScanning();

            return sabaDangerLevel.CustSabaDangerLevel.Any();
        }

        public ISabaDangerLevelServices UpdateSabaDangerLevelToMainDB(ISentryClient sentryClient, string serviceName, int batchSize)
        {
            if (sabaDangerLevel.CustSabaDangerLevel.Any())
            {
                var sabaDangerLevelChunk = sabaDangerLevel.CustSabaDangerLevel.Chunk(batchSize);

                foreach (var sabaDangerLevelBatch in sabaDangerLevelChunk)
                {
                    sabaDangerLevel.UpdatedResult = sabaDangerLevelDataAccess.UpdateDabaDangerLevelToMainDB(sabaDangerLevelBatch);

                    if (!sabaDangerLevel.UpdatedResult.IsSuccess)
                    {
                        Utilities.LogSentryError(serviceName, sabaDangerLevel.UpdatedResult.ErrMsg, sabaDangerLevel.CustSabaDangerLevel, new Exception(sabaDangerLevel.UpdatedResult.ErrMsg), sentryClient);

                        return this;
                    }
                }
            }

            return this;
        }

        public ISabaDangerLevelServices UpdateLogSabaDangerLevel()
        {
            if (sabaDangerLevel.CustSabaDangerLevel.Any() && sabaDangerLevel.UpdatedResult.IsSuccess)
            {
                var custDangerLevelJson = JsonConvert.SerializeObject(sabaDangerLevel.CustSabaDangerLevel);

                sabaDangerLevelDataAccess.UpdateLogSabaDangerLevel(custDangerLevelJson);
            }

            return this;
        }

        public bool UpdateLastScan()
        {
            if (sabaDangerLevel.UpdatedResult.IsSuccess)
            {
                systemParameterServices.UpdateTimingExecution(sabaDangerLevel.LastHistoryID.ToString(), SystemParameters.SabaDangerLevel_LastHistoryID);

                return true;
            }

            return false;
        }

        #region For Initialzing Data

        public ISabaDangerLevelServices StartForInitialzingData()
        {
            initSabaDangerLevel = new SabaDangerLevelInitResultEntity();

            return this;
        }

        public bool GetSabaDangerLevelForInitialization()
        {
            initSabaDangerLevel = sabaDangerLevelDataAccess.GetSabaDangerLevelForInitialization();

            return initSabaDangerLevel.LastCategoryID != -1;
        }

        public ISabaDangerLevelServices UpdateSabaDangerLevelToMainDBForInitialization(ISentryClient sentryClient, string serviceName, int batchSize)
        {
            if (initSabaDangerLevel.CustSabaDangerLevel.Any())
            {
                var initabaDangerLevelChunk = initSabaDangerLevel.CustSabaDangerLevel.Chunk(batchSize);

                foreach (var initsabaDangerLevelBatch in initabaDangerLevelChunk)
                {
                    initSabaDangerLevel.UpdatedResult = sabaDangerLevelDataAccess.UpdateDabaDangerLevelToMainDB(initsabaDangerLevelBatch);

                    if (!initSabaDangerLevel.UpdatedResult.IsSuccess)
                    {
                        Utilities.LogSentryError(serviceName, sabaDangerLevel.UpdatedResult.ErrMsg, initSabaDangerLevel.CustSabaDangerLevel, null, sentryClient);

                        return this;
                    }
                }
            }

            return this;
        }

        public ISabaDangerLevelServices UpdateLogSabaDangerLevelForInitialization()
        {
            if (initSabaDangerLevel.CustSabaDangerLevel.Any() && initSabaDangerLevel.UpdatedResult.IsSuccess)
            {
                var custDangerLevelJson = JsonConvert.SerializeObject(initSabaDangerLevel.CustSabaDangerLevel);

                sabaDangerLevelDataAccess.UpdateLogSabaDangerLevel(custDangerLevelJson);
            }

            return this;
        }

        public bool UpdateLastScanForInitialization()
        {
            if (initSabaDangerLevel.UpdatedResult.IsSuccess)
            {
                systemParameterServices.UpdateTimingExecution(initSabaDangerLevel.LastCategoryID.ToString(), SystemParameters.SabaDangerLevel_LastCategoryID);

                systemParameterServices.UpdateTimingExecution(initSabaDangerLevel.LastCustID.ToString(), SystemParameters.SabaDangerLevel_LastCustID);

                return true;
            }

            return false;
        }

        #endregion For Initialzing Data
    }
}