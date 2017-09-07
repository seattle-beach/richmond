using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Richmond
{
    public class CoffeeController
    {
        [HttpGet("coffee")]
        public IActionResult Coffee()
        {
            var httpClientWrapper = new HttpClientWrapper();
            var task = httpClientWrapper.GetAsync("https://cafe.cfapps.io/status");
            HttpResponseMessage responseMessage = task.Result;
            string payload = responseMessage.Content.ReadAsStringAsync().Result;
            var coffeResult = JsonConvert.DeserializeObject<List<Coffee>>(payload);
            var coffee = new CoffeeResponse
            {
                 Coffees = coffeResult
            };
            return new OkObjectResult(coffee);
        }
    }
}