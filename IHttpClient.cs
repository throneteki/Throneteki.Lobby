namespace Throneteki.LobbyNode
{
    using System.Threading.Tasks;

    public interface IHttpClient
    {
        Task<T> GetJsonObjectAsync<T>(string url)
            where T : class;
    }
}
