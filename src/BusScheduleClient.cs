using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace Richmond
{
    public interface IBusScheduleClient
    {
        IEnumerable<Bus> Fetch();
    }

    public class BusScheduleClient : IBusScheduleClient
    {
        private readonly IHttpClientWrapper httpClientWrapper;

        public BusScheduleClient(IHttpClientWrapper httpClientWrapper)
        {
            this.httpClientWrapper = httpClientWrapper;
        }

        public IEnumerable<Bus> Fetch()
        {
            var task = httpClientWrapper.GetAsync("http://weatherbus-bus-dev.cfapps.io/api/v1/departures?stopId=1_1535");
            HttpResponseMessage responseMessage = task.Result;
            string payload = responseMessage.Content.ReadAsStringAsync().Result;
            var weatherBusResult = JsonConvert.DeserializeObject<WeatherBusResult>(payload);

            return weatherBusResult.data.Select(d => new Bus(){ ShortName = d.RouteShortName});
        }

        class WeatherBusResult
        {
            public IEnumerable<WeatherBusDatum> data;
        }

        class WeatherBusDatum
        {
            public string RouteShortName;
        }
    }
}