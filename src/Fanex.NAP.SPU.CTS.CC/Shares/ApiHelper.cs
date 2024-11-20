namespace Fanex.NAP.SPU.CTS.Shares
{
    using Fanex.NAP.SPU.CTS.CC.Shares.Models;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text;
    using System.Threading.Tasks;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4005:\"System.Uri\" arguments should be used instead of strings", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S112:General or reserved exceptions should never be thrown", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Info Code Smell", "S4136:All 'Post' method overloads should be adjacent.", Justification = "<NotHandle>")]
    public static class ApiHelper
    {
        private static APIHelperConfig _config;
        public static void Setup(APIHelperConfig config)
        {
            _config = config;
        }

        [Obsolete("Too many generic and parameter")]
        public static async Task<TResult> Post<TParam, TResult>(string url, TParam param, int environment, int mailingListId)
        {
            using (var web = new HttpClient())
            {
                var response = await web.PostAsJsonAsync(url, param).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    return default(TResult);
                }

                return await response.Content.ReadFromJsonAsync<TResult>().ConfigureAwait(false);
            }
        }

        public static async Task<TResult> Post<TResult>(string url, object param, double timeOutbySeconds = 120)
        {
            using (var web = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(param);

                var buffer = Encoding.UTF8.GetBytes(content);

                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                web.Timeout = TimeSpan.FromSeconds(timeOutbySeconds);

                var response = await web.PostAsync(url, byteContent).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }

        public static async Task<TResult> PostWithProxy<TResult>(string url, object param)
        {
            var proxy = new WebProxy
            {
                Address = new Uri(_config?.Proxy)
            };

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
            };

            using (var web = new HttpClient(handler: httpClientHandler, disposeHandler: true))
            {
                var content = JsonConvert.SerializeObject(param);

                var buffer = Encoding.UTF8.GetBytes(content);

                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await web.PostAsync(url, byteContent).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }

        public static async Task Post(string url, object param)
        {
            using (var web = new HttpClient())
            {
                await web.PostAsJsonAsync(url, param).ConfigureAwait(false);
            }
        }

        public static async Task<TResult> Get<TResult>(string url)
        {
            using (var web = new HttpClient())
            {
                var response = await web.GetAsync(url).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }
    }
}