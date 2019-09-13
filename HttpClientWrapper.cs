namespace Throneteki.LobbyNode
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient httpClient;

        public HttpClientWrapper()
        {
            httpClient = new HttpClient();
        }

        public async Task<T> GetJsonObjectAsync<T>(string url)
            where T : class
        {
            string response;

            try
            {
                response = await httpClient.GetStringAsync(url);
            }
            catch (Exception)
            {
                return null;
            }

            return response == null ? null : JsonConvert.DeserializeObject<T>(response);
        }
    }
}
