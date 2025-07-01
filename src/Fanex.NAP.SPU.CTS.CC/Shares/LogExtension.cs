namespace Fanex.NAP.SPU.CTS.Shares
{
    using System;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;

    public static class LogExtension
    {
        private static bool IsLogSentry = true;

        public static void ConfigByPassForUnitTest()
        {
            IsLogSentry = false;
        }

        public static void LogInfo(this string message)
        {
            if (IsLogSentry)
            {
                LogService.Info(message);
            }
        }

        public static void LogError(this Exception ex, string message = null)
        {
            if (IsLogSentry)
            {
                var logMessage = ex.CreateLogMessage(message);

                LogService.Error(logMessage);
            }
        }

        public static void LogError(this string message, Exception ex)
        {
            if (IsLogSentry)
            {
                LogService.Error(message, ex);
            }
        }

        public static void LogError(this string message)
        {
            if (IsLogSentry)
            {
                LogService.Error(message);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Code Smell", "S6566:Use DateTimeOffset instead of DateTime", Justification = "Legacy code or specific reason")]
        public static string CreateLogMessage(this Exception ex, string message = null)
        {
            return new LogMessage(ex.Message)
                .AddMessage(message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                .AddMessage($"{DateTime.Now}").ToString(); // NOSONAR
        }
    }
}
