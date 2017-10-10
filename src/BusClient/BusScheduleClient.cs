using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Richmond.BusClient
{
    public interface IBusScheduleClient
    {
        WeatherBusResult Fetch();
    }

    public class BusScheduleClient : IBusScheduleClient
    {
        private readonly IHttpClientWrapper httpClientWrapper;
        private readonly ILogger logger;

        public BusScheduleClient(IHttpClientWrapper httpClientWrapper, ILogger logger)
        {
            this.httpClientWrapper = httpClientWrapper;
            this.logger = logger;
        }

        public WeatherBusResult Fetch()
        {
            var task = httpClientWrapper.GetAsync("http://weatherbus-bus-dev.cfapps.io/api/v1/departures?stopId=1_623");
            HttpResponseMessage responseMessage = task.Result;
            string payload = responseMessage.Content.ReadAsStringAsync().Result;
            var busResult = JsonConvert.DeserializeObject<WeatherBusResult>(payload);

            if (!responseMessage.IsSuccessStatusCode || busResult == null || busResult.Data == null)
            {
                this.logger.Log("bus response null!");
                this.logger.Log("HttpResponseMessage: " + responseMessage.ToString());
                this.logger.Log("Payload: " + payload);
            }

            return busResult;
        }
    }

    public class WeatherBusResult
    {
        public IEnumerable<WeatherBusDatum> Data = null;
    }

    public class WeatherBusDatum
    {
        public string RouteShortName = null;
        public string Headsign = null;
        public long PredictedTime = 0;
        public long ScheduledTime = 0;
    }
}
