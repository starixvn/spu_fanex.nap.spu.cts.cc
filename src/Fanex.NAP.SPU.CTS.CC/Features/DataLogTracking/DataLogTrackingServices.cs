namespace Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDataLogTrackingServices
    {
        void LogInputData(LogInputViewEntity content, FunctionId funcId, string serviceName);

        void LogErrorData(Exception exception, string serviceName);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Datetime Code Smell", "S6566:Prefer using DateTimeOffset instead of DateTime", Justification = "<NotHandle>")]
    public class DataLogTrackingServices : IDataLogTrackingServices
    {
        private readonly IDataLogTrackingDataAccess dataLogTrackingDataAccess;

        public DataLogTrackingServices(IDataLogTrackingDataAccess dataLogTrackingDataAccess)
        {
            this.dataLogTrackingDataAccess = dataLogTrackingDataAccess;
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
                        Utilities.LogSentryError(serviceName, "InsertInputData fail", (content, funcId), new Exception(result.ErrorMsg));
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "InsertInputData fail", (content, funcId), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "<Pending>")]
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
                Utilities.LogSentryError(serviceName, "InsertErrorData fail", exception, ex);
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