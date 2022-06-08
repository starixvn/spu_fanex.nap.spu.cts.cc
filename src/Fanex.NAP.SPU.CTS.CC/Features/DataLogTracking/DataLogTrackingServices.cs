namespace Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDataLogTrackingServices
    {
        void LogInputData(LogInputViewEntity content, FunctionId funcId, string serviceName);
    }

    public class DataLogTrackingServices : IDataLogTrackingServices
    {
        private readonly IDataLogTrackingDataAccess dataLogTrackingDataAccess;
        private readonly ISentryClient sentryClient;

        public DataLogTrackingServices(IDataLogTrackingDataAccess dataLogTrackingDataAccess, ISentryClient sentryClient)
        {
            this.dataLogTrackingDataAccess = dataLogTrackingDataAccess;
            this.sentryClient = sentryClient;
        }

        public void LogInputData(LogInputViewEntity content, FunctionId funcId, string serviceName)
        {
            try
            {
                var inputData = MappingLogInputData(content);

                if (inputData != null && inputData.Any())
                {
                    var result = dataLogTrackingDataAccess.LogInputData(inputData, funcId);

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

        private static IEnumerable<LogInputDataEntity> MappingLogInputData(LogInputViewEntity content)
        {
            var logInputData = new List<LogInputDataEntity>();

            if (content != null && content.RobotUser != null && content.RobotUser.Any())
            {
                content.RobotUser.ToList().ForEach(c =>
                {
                    logInputData.Add(new LogInputDataEntity
                    {
                        CustId = c.CustID,
                        InputValue = new { c.RobotCounter, c.CategoryID, c.CreatedBy }.ToString()
                    });
                });
            }

            return logInputData;
        }
    }
}