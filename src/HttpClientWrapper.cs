using System.Net.Http;
using System.Threading.Tasks;

namespace Richmond
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }

    public class HttpClientWrapper : IHttpClientWrapper
    {
        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            var client = new HttpClient();
            return client.GetAsync(requestUri);
        }
    }
}