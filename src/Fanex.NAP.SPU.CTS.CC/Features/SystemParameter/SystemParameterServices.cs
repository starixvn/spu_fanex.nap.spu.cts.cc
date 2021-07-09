namespace Fanex.NAP.SPU.CTS.Features.SystemParameter
{
    using System;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ISystemParameterServices
    {
        Int64 GetLastScannedId(SystemParameters paraID);

        DateTime? GetLastScannedDateTime(SystemParameters paraID);

        bool UpdateTimingExecution(string paraValue, SystemParameters paramId);
    }

    public class SystemParameterServices : ISystemParameterServices
    {
        private readonly ISystemParameterDataAccess systemParameterDataAccess;

        public SystemParameterServices(ISystemParameterDataAccess systemParameterDataAccess)
        {
            this.systemParameterDataAccess = systemParameterDataAccess;
        }

        public Int64 GetLastScannedId(SystemParameters paraID)
        {
            var lastScannedTransID = systemParameterDataAccess.GetPreviousTimingExecution(paraID);

            return lastScannedTransID != null ? Int64.Parse(lastScannedTransID?.ParameterValue) : 0;
        }

        public DateTime? GetLastScannedDateTime(SystemParameters paraID)
        {
            var lastScannedTransID = systemParameterDataAccess.GetPreviousTimingExecution(paraID);

            return lastScannedTransID != null ? DateTime.Parse(lastScannedTransID?.ParameterValue) : null;
        }

        public bool UpdateTimingExecution(string paraValue, SystemParameters paramId)
        {
            if (!string.IsNullOrEmpty(paraValue))
            {
                systemParameterDataAccess.UpdateTimingExecution(paraValue, paramId);
            }

            return true;
        }
    }
}