using Microsoft.AspNetCore.Mvc;
using System;

namespace Richmond
{
    public class BusScheduleController
    {
        private readonly IBusScheduleClient busScheduleClient;

        public BusScheduleController(IBusScheduleClient busScheduleClient)
        {
            this.busScheduleClient = busScheduleClient;
        }

        [HttpGet("buses")]
        public IActionResult Buses()
        {
            try
            {
                var schedule = new BusScheduleResponse
                {
                    Buses = this.busScheduleClient.Fetch()
                };

                return new OkObjectResult(schedule);
            }
            catch (Exception e)
            {
                // TODO: inject this dependency and squelch in tests
                System.Console.Out.WriteLine(e.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
