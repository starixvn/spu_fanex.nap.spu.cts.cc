namespace Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Entities;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWRobotImpervaService
    {
        long GetLastScannedID(CTS_DC_SystemParameters paraID);

        void UpdateLastScannedID(CTS_DC_SystemParameters para, long maxLogID);

        TWRobotImpervaGetModel GetTWRobotImperva(long lastLogID);

        void InsertTWRobotImperva(IEnumerable<TWRobotImpervaGetEntity> robots);
    }

    public class TWRobotImpervaService : ITWRobotImpervaService
    {
        private readonly ITWRobotImpervaDataAccess twRobotDataAccess;
        private readonly ISystemParameterDataAccess sysDataAccess;

        public TWRobotImpervaService(ITWRobotImpervaDataAccess twRobotDataAccess, ISystemParameterDataAccess sysDataAccess)
        {
            this.twRobotDataAccess = twRobotDataAccess;
            this.sysDataAccess = sysDataAccess;
        }

        public long GetLastScannedID(CTS_DC_SystemParameters paraID)
        {
            var lastValue = sysDataAccess.GetPreviousTimingExecution(paraID);
            
            return lastValue != null ? long.Parse(lastValue?.ParameterValue) : -1;
        }

        public void UpdateLastScannedID(CTS_DC_SystemParameters para, long maxLogID)
        {
            if (maxLogID > 0)
            {
                sysDataAccess.UpdateTimingExecution(maxLogID.ToString(), para);
            }
        }

        public TWRobotImpervaGetModel GetTWRobotImperva(long lastLogID)
        {
            return twRobotDataAccess.GetTWRobotImperva(lastLogID);
        }

        public void InsertTWRobotImperva(IEnumerable<TWRobotImpervaGetEntity> robots)
        {
            if (robots.Any())
            {
                var robotInserts = robots.Select(x => new TWRobotImpervaInsertEntity
                {
                    LogID = x.logid,
                    CustID = x.CustId,
                    Platform = x.Platform,
                    CreateTime = x.CreateTime
                });

                twRobotDataAccess.InsertTWRobotImperva(JsonConvert.SerializeObject(robotInserts));
            }           
        }

    }
}
