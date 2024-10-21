namespace Fanex.NAP.SPU.CTS.CC.Features.SystemParameter
{
    using System;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ISystemParameterServices
    {
        Int64 GetLastScannedId(CTS_SystemParameters paraID);

        UInt64 GetLastScannedId2(CTS_SystemParameters paraID);

        DateTime? GetLastScannedDateTime(CTS_SystemParameters paraID);

        bool UpdateTimingExecution(string paraValue, CTS_SystemParameters paramId);     
    }

    public class SystemParameterServices : ISystemParameterServices
    {
        private readonly ISystemParameterDataAccess systemParameterDataAccess;

        public SystemParameterServices(ISystemParameterDataAccess systemParameterDataAccess)
        {
            this.systemParameterDataAccess = systemParameterDataAccess;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "<Pending>")]
        public Int64 GetLastScannedId(CTS_SystemParameters paraID)
        {
            var lastScannedId = systemParameterDataAccess.GetPreviousTimingExecutionForCTS(paraID);

            return lastScannedId != null ? Int64.Parse(lastScannedId?.ParameterValue) : 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "<Pending>")]
        public UInt64 GetLastScannedId2(CTS_SystemParameters paraID)
        {
            var lastScannedId = systemParameterDataAccess.GetPreviousTimingExecutionForCTS(paraID);

            return lastScannedId != null ? UInt64.Parse(lastScannedId?.ParameterValue) : 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S6580:Use a format provider when parsing date and time", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "<Pending>")]
        public DateTime? GetLastScannedDateTime(CTS_SystemParameters paraID)
        {
            var lastScannedTransID = systemParameterDataAccess.GetPreviousTimingExecutionForCTS(paraID);

            return lastScannedTransID != null ? DateTime.Parse(lastScannedTransID?.ParameterValue) : null;
        }

        public bool UpdateTimingExecution(string paraValue, CTS_SystemParameters paramId)
        {
            if (!string.IsNullOrEmpty(paraValue))
            {
                systemParameterDataAccess.UpdateTimingExecutionForCTS(paraValue, paramId);
            }

            return true;
        }
    }
}