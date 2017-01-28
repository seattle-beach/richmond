using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Richmond
{
    public class BusScheduleController
    {
        private IBusService busService;
        private IDateProvider dateProvider;

        public BusScheduleController(IBusService busService, IDateProvider dateProvider)
        {
            this.busService = busService;
            this.dateProvider = dateProvider;
        }

        [HttpGet("buses")]
        public IActionResult Buses()
        {
            BusServiceResponse busServiceResponse = busService.BusesForStop();

            var epochNow = dateProvider.EpochNow();

            var busResponse = new BusResponse
            {
                Buses = busServiceResponse.Data.Select(b => ToBusResponseBus(b, epochNow))
            };

            return new OkObjectResult(busResponse);
        }

        private static BusResponse.Bus ToBusResponseBus(BusServiceResponse.Bus serviceBus, ulong now)
        {
            var eta = (serviceBus.PredictedTime - now) / (1000 * 60);
            var status = "early";

            return new BusResponse.Bus
            {
                ShortName = serviceBus.RouteShortName,
                LongName = serviceBus.Headsign,
                Eta = (int)eta,
                Status = status
            };
        }
    }
}
