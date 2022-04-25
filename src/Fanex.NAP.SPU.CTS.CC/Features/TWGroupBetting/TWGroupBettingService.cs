namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWGroupBettingService
    {
        Int64 ScanTicket(DateTime fromTime, DateTime toTime, int step, Int64 lastTransID);

        void CountGroupBettingTicket();

        void CountTotalTicket();

        DateTime GetLastScanTime(SystemParameters paraID);

        DateTime GetLastTimeCountTotalTicket(SystemParameters paraID);

        void UpdateLastScanTime(DateTime lastScannedDate, SystemParameters paraID);

        IEnumerable<TWGroupBettingRule> GetAllRules();

        void SyncGroupBettingTicketToDB062(IEnumerable<TWGroupBettingTicket> gbTickets);

        Int64 GetLastTransID(SystemParameters paraID);

        void UpdateMaxTransID(Int64 transId, SystemParameters paraID);
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

        public DateTime GetLastScanTime(SystemParameters paraID)
        {
            var lastScannedDate = systemParameterDataAccess.GetPreviousTimingExecution(paraID);
            var defaultDate = new DateTime(2022, 1, 1);
            return lastScannedDate != null ? DateTime.Parse(lastScannedDate?.ParameterValue) : defaultDate;
        }

        public Int64 GetLastTransID(SystemParameters paraID)
        {
            var lastScannedDate = systemParameterDataAccess.GetPreviousTimingExecution(paraID);
            Int64 defaultValue = 0;
            return lastScannedDate != null ? Int64.Parse(lastScannedDate?.ParameterValue) : defaultValue;
        }

        public void UpdateMaxTransID(Int64 transId, SystemParameters paraID)
        {
            systemParameterDataAccess.UpdateTimingExecution(transId.ToString(), paraID);
        }

        public void UpdateLastScanTime(DateTime lastScannedDate, SystemParameters paraID)
        {
            systemParameterDataAccess.UpdateTimingExecution(lastScannedDate.ToString("yyyy-MM-dd HH:mm:ss.fff"), paraID);
        }

        public DateTime GetLastTimeCountTotalTicket(SystemParameters paraID)
        {
            var lastScannedDate = systemParameterDataAccess.GetPreviousTimingExecution(paraID);
            var defaultDate = new DateTime(2022, 1, 1);
            return lastScannedDate != null ? DateTime.Parse(lastScannedDate?.ParameterValue) : defaultDate;
        }

        public Int64 ScanTicket(DateTime fromTime, DateTime toTime, int step, Int64 lastTransID) 
        {
            return twGroupBettingDataAccess.ScanTicket(fromTime, toTime, step, lastTransID);
        }

        public void CountGroupBettingTicket()
        {
            try
            {
                twGroupBettingDataAccess.CountGroupBettingTicket();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                twGroupBettingDataAccess.CountGroupBettingTicketForDB062();
            }
            
        }

        public void CountTotalTicket() 
        {
            twGroupBettingDataAccess.CountTotalTicket();
            twGroupBettingDataAccess.CountTotalTicketDB062();
        }

        public IEnumerable<TWGroupBettingRule> GetAllRules()
        {
            return twGroupBettingDataAccess.GetAllRules();
        }

        public void SyncGroupBettingTicketToDB062(IEnumerable<TWGroupBettingTicket> gbTickets)
        {
            twGroupBettingDataAccess.SyncGroupBettingTicketToDB062(gbTickets);
        }
    }
}