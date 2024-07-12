namespace Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDataLogTrackingServices
    {
        void LogInputData(LogInputViewEntity content, FunctionId funcId, string serviceName);

        void LogErrorData(Exception exception, string serviceName);
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
                        Utilities.LogSentryError(serviceName, "InsertInputData fail", (content, funcId), new Exception(result.ErrorMsg), sentryClient);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "InsertInputData fail", (content, funcId), ex, sentryClient);
            }
        }

        public void LogErrorData(Exception exception, string serviceName)
        {
            try
            {                
                var logInfo = new LogErrorDataEntity
                {
                    Message = new LogMessage(exception.Message)
                                   .AddMessage($"{serviceName}")
                                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                                   .AddMessage($"{ DateTime.Now}").ToString(),
                    FromFlow = serviceName,
                    SPName = DatabaseInfoGetters.GetDbExceptionInfo(exception)?.Where(c => c.Key == "SpName")?.FirstOrDefault().Value?.ToString(),
                    Parameters = DatabaseInfoGetters.GetDbExceptionInfo(exception)?.Where(c => c.Key == "Parameters")?.FirstOrDefault().Value?.ToString(),
                };

                dataLogTrackingDataAccess.LogErrorData(logInfo);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "InsertErrorData fail", exception, ex, sentryClient);
            }
        }


        #region Private Methods

        private static IEnumerable<LogInputDataEntity> MappingLogInputData(LogInputViewEntity content)
        {
            var logInputData = new List<LogInputDataEntity>();

            if (content != null && content.RobotUser != null && content.RobotUser.Any())
            {
                content.RobotUser.ToList().ForEach(c =>
                {
                    logInputData.Add(new LogInputDataEntity
                    {
                        CustId = c.CustId,
                        InputValue = new { c.Counter }.ToString()
                    });
                });
            }

            return logInputData;
        }

        #endregion
    }
}