namespace Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWGroupBettingService
    {
        IEnumerable<TWGroupBettingTicket> ScanTicket(DateTime fromTime, DateTime toTime, int step);

        void CountGroupBettingTicket();

        DateTime CountTotalTicket(DateTime lastUpdatedTotalTicketTime);

        void CountGroupBettingTicketForDB062();

        DateTime GetLastScanTime(SystemParameters paraID);

        DateTime GetLastTimeCountTotalTicket(SystemParameters paraID);
        
        void UpdateLastScanTime(DateTime lastScannedDate, SystemParameters paraID);

        IEnumerable<TWGroupBettingRule> GetAllRules();

        void SyncGroupBettingTicketToDB062(IEnumerable<TWGroupBettingTicket> gbTickets);
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

        public IEnumerable<TWGroupBettingTicket> ScanTicket(DateTime fromTime, DateTime toTime, int step) 
        {
            return twGroupBettingDataAccess.ScanTicket(fromTime, toTime, step);
        }

        public void CountGroupBettingTicket()
        {
            twGroupBettingDataAccess.CountGroupBettingTicket();
        }
        
        public void CountGroupBettingTicketForDB062()
        {
            twGroupBettingDataAccess.CountGroupBettingTicketForDB062();
        }

        public DateTime CountTotalTicket(DateTime lastUpdatedTotalTicketTime) 
        {
            return twGroupBettingDataAccess.CountTotalTicket(lastUpdatedTotalTicketTime);
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