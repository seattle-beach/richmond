using System.Net.Http;
using System.Threading.Tasks;

namespace Richmond.Tests
{
    public class MockHttpClientWrapper : IHttpClientWrapper
    {
        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return Task.FromResult(new HttpResponseMessage { Content = new StringContent(@"{""data"":[{""routeShortName"":""99"",""headsign"":""Sand Point East Green Lake"",""predictedTime"":1485476239000,""scheduledTime"":1485476239000}]}")});
        }
    }
}
