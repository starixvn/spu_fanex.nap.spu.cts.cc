namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWGroupBettingService
    {
        TWGroupBetting_ScanTicketCriteria ScanTicket(TWGroupBetting_ScanTicketCriteria criteria);

        long UpdateTotalTWGBTicket(int batchSize, long LastSequenceId);

        IEnumerable<TWGroupBettingRule> GetAllRules();

        Int64 GetConfig_Int64(Main_SystemParameters paraID);

        void UpdateConfig_Int64(Int64 value, Main_SystemParameters paraID);

        int GetConfig_Int(Main_SystemParameters paraID);

        void UpdateConfig_Int(int value, Main_SystemParameters paraID);

        int? GetConfigWithoutDefault(Main_SystemParameters paraID);
    }

    public class TWGroupBettingService : ITWGroupBettingService
    {
        private readonly ITWGroupBettingDataAccess twGroupBettingDataAccess;
        private readonly ISystemParameterDataAccess systemParameterDataAccess;

        public TWGroupBettingService(ITWGroupBettingDataAccess twGroupBettingDataAccess,
            ISystemParameterDataAccess systemParameterDataAccess)
        {
            this.twGroupBettingDataAccess = twGroupBettingDataAccess;
            this.systemParameterDataAccess = systemParameterDataAccess;
        }

        public Int64 GetConfig_Int64(Main_SystemParameters paraID)
        {
            var lastScannedDate = systemParameterDataAccess.GetPreviousTimingExecution(paraID);
            Int64 defaultValue = 0;
            return lastScannedDate != null ? Int64.Parse(lastScannedDate?.ParameterValue) : defaultValue;
        }

        public int GetConfig_Int(Main_SystemParameters paraID)
        {
            var lastScannedDate = systemParameterDataAccess.GetPreviousTimingExecution(paraID);
            int defaultValue = 0;
            return lastScannedDate != null ? int.Parse(lastScannedDate?.ParameterValue) : defaultValue;
        }

        public int? GetConfigWithoutDefault(Main_SystemParameters paraID)
        {
            var lastScannedDate = systemParameterDataAccess.GetPreviousTimingExecution(paraID);
            return lastScannedDate != null ? int.Parse(lastScannedDate?.ParameterValue) : null;
        }

        public void UpdateConfig_Int64(Int64 value, Main_SystemParameters paraID)
        {
            systemParameterDataAccess.UpdateTimingExecution(value.ToString(), paraID);
        }

        public void UpdateConfig_Int(int value, Main_SystemParameters paraID)
        {
            systemParameterDataAccess.UpdateTimingExecution(value.ToString(), paraID);
        }

        public TWGroupBetting_ScanTicketCriteria ScanTicket(TWGroupBetting_ScanTicketCriteria criteria)
        {
            return twGroupBettingDataAccess.ScanTicket(criteria);
        }

        public long UpdateTotalTWGBTicket(int batchSize, long LastSequenceId)
        {
            return twGroupBettingDataAccess.UpdateTotalTWGBTicket(batchSize, LastSequenceId);
        }

        public IEnumerable<TWGroupBettingRule> GetAllRules()
        {
            return twGroupBettingDataAccess.GetAllRules();
        }
    }
}