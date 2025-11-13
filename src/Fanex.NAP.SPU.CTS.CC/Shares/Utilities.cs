namespace Fanex.NAP.SPU.CTS.Shares
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.Notification.Services;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Datetime Code Smell", "S6566:Prefer using DateTimeOffset instead of DateTime", Justification = "<NotHandle>")]
    public static class Utilities
    {
        public static IEnumerable<IEnumerable<TValue>> Chunk2<TValue>(
            this IEnumerable<TValue> values,
            Int32 chunkSize)
        {
            return values
                .Select((v, i) => new { v, groupIndex = i / chunkSize })
                .GroupBy(x => x.groupIndex)
                .Select(g => g.Select(x => x.v));
        }

        public static async Task<TResult> Post<TParam, TResult>(string url, TParam param)
        {
            using (var web = new HttpClient { Timeout = new TimeSpan(0, 5, 0) })
            {
                var response = await web.PostAsJsonAsync(url, param).ConfigureAwait(false);

                return await response.Content.ReadFromJsonAsync<TResult>().ConfigureAwait(false);
            }
        }

        public static async Task<TResult> Post<TResult>(string url)
        {
            using (var web = new HttpClient { Timeout = new TimeSpan(0, 5, 0) })
            {
                var response = await web.PostAsJsonAsync(url, string.Empty).ConfigureAwait(false);

                return await response.Content.ReadFromJsonAsync<TResult>().ConfigureAwait(false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Bug", "S2955:Generic parameters not constrained to reference types should not be compared to \"null\"", Justification = "<Pending>")]
        public static void LogSentryInfo<T>(string serviceName, string message, T data, bool isWriteLog)
        {
            if (isWriteLog)
            {
                var logMessage = $"{serviceName} - {message} {(data != null ? " - Data:" + JsonConvert.SerializeObject(data) : "")} - {DateTime.Now}";

                logMessage.LogError();
            }
        }

        public static void LogSentryError<T>(string serviceName, string message, T data, Exception ex, string dataType = "Data")
        {
            var logMessage = new LogMessage(ex.Message)
                .AddMessage($"{serviceName}")
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                .AddMessage($"{message} - {dataType}: {JsonConvert.SerializeObject(data)} - {DateTime.Now}").ToString();

            logMessage.LogError();
        }

        public static void LogSentryError(string serviceName, string message, Exception ex)
        {
            var logMessage = new LogMessage(ex.Message)
                .AddMessage($"{serviceName}")
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                .AddMessage($"{message} - {DateTime.Now}").ToString();

            logMessage.LogError();
        }

        public static string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static void NotifyChannelMessage(string message, string systemName, string environment, string groupId)
        {
            var notificationService = new NotificationService();
            string proxy = new HashSet<string> { "PROD", "KLDR" }.Contains(environment) ? "iusqd.gamora.prod:8801" : null;
            notificationService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", groupId, "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken", proxy)
               .Message(new MessageModel(systemName, DateTime.Now, environment, message))
               .SendWithoutDelay();
        }

        public static bool FlatMap(this bool isAction, Func<bool> func)
        {
            return isAction && func();
        }

        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}