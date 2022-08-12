namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Newtonsoft.Json;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ISabaDangerLevelServices
    {
        ISabaDangerLevelServices Start();

        ISabaDangerLevelServices GetSabaDangerLevelForScanning();

        ISabaDangerLevelServices UpdateLogSabaDangerLevel();

        ISabaDangerLevelServices UpdateLastScan();

        #region For Initialzing Data

        ISabaDangerLevelServices StartForInitialzingData();

        ISabaDangerLevelServices GetSabaDangerLevelForInitializing();

        ISabaDangerLevelServices UpdateLogSabaDangerLevelForInitializing();

        ISabaDangerLevelServices UpdateLastScanForInitializing();

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

        public ISabaDangerLevelServices GetSabaDangerLevelForScanning()
        {
            sabaDangerLevel = sabaDangerLevelDataAccess.GetSabaDangerLevelForScanning();

            return this;
        }

        public ISabaDangerLevelServices UpdateLogSabaDangerLevel()
        {
            if (sabaDangerLevel.CustSabaDangerLevel.Any())
            {
                var custDangerLevelJson = JsonConvert.SerializeObject(sabaDangerLevel.CustSabaDangerLevel);

                sabaDangerLevelDataAccess.UpdateLogSabaDangerLevel(custDangerLevelJson);
            }

            return this;
        }

        public ISabaDangerLevelServices UpdateLastScan()
        {
            systemParameterServices.UpdateTimingExecution(sabaDangerLevel.LastHistoryID.ToString(), SystemParameters.SabaDangerLevel_LastHistoryID);

            return this;
        }

        #region For Initialzing Data

        public ISabaDangerLevelServices StartForInitialzingData()
        {
            initSabaDangerLevel = new SabaDangerLevelInitResultEntity();

            return this;
        }

        public ISabaDangerLevelServices GetSabaDangerLevelForInitializing()
        {
            initSabaDangerLevel = sabaDangerLevelDataAccess.GetSabaDangerLevelForInitializing();

            return this;
        }

        public ISabaDangerLevelServices UpdateLogSabaDangerLevelForInitializing()
        {
            if (initSabaDangerLevel.CustSabaDangerLevel.Any())
            {
                var custDangerLevelJson = JsonConvert.SerializeObject(initSabaDangerLevel.CustSabaDangerLevel);

                sabaDangerLevelDataAccess.UpdateLogSabaDangerLevel(custDangerLevelJson);
            }

            return this;
        }

        public ISabaDangerLevelServices UpdateLastScanForInitializing()
        {
            systemParameterServices.UpdateTimingExecution(initSabaDangerLevel.LastCategoryID.ToString(), SystemParameters.SabaDangerLevel_LastCategoryID);

            systemParameterServices.UpdateTimingExecution(initSabaDangerLevel.LastCustID.ToString(), SystemParameters.SabaDangerLevel_LastCustID);

            return this;
        }

        #endregion For Initialzing Data
    }
}