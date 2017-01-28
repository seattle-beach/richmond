using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace Richmond
{
    public interface IBusService
    {
        BusServiceResponse BusesForStop();
    }

    public class BusService : IBusService
    {
        private readonly HttpMessageHandler httpMessageHandler;
        private string busEndpoint;

        public BusService(HttpMessageHandler httpMessageHandler, string busEndpoint)
        {
            this.httpMessageHandler = httpMessageHandler ?? new HttpClientHandler();
            this.busEndpoint = busEndpoint ?? Environment.GetEnvironmentVariable("BUS_PATH");
        }

        public BusService() : this(null, null)
        {}

        public BusServiceResponse BusesForStop()
        {
            HttpClient client = new HttpClient(this.httpMessageHandler);
            var uri = this.busEndpoint + "/departures?stopId=1_1535";
            var payload = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<BusServiceResponse>(payload);
        }
    }
}