using Microsoft.AspNetCore.Mvc;
using Richmond.BusClient;
using System;
using System.Linq;

namespace Richmond
{
    public class BusScheduleController
    {
        private readonly IBusScheduleClient busScheduleClient;
        private readonly IDateProvider dateProvider;
        private readonly IBusFactory busFactory;
        private readonly ILogger logger;

        public BusScheduleController(
            IBusScheduleClient busScheduleClient,
            IDateProvider dateProvider,
            IBusFactory busFactory,
            ILogger logger)
        {
            this.busScheduleClient = busScheduleClient;
            this.dateProvider = dateProvider;
            this.busFactory = busFactory;
            this.logger = logger;
        }

        [HttpGet("buses")]
        public IActionResult Buses()
        {
            try
            {
                var busResponse = this.busScheduleClient.Fetch();

                if (busResponse == null || busResponse.Data == null)
                {
                    this.logger.Log("bus response null!");
                    this.logger.Log("Payload: " + Newtonsoft.Json.JsonConvert.SerializeObject(busResponse));
                }

                var now = this.dateProvider.EpochNow();
                var buses = busResponse.Data
                    .Select(b => this.busFactory.FromWeatherBus(b, now))
                    .Where(b => string.Equals("Link light rail", b.ShortName))
                    .Take(2);

                var schedule = new BusScheduleResponse
                {
                    Buses = buses
                };

                return new OkObjectResult(schedule);
            }
            catch (Exception e)
            {
                this.logger.Log(e.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
