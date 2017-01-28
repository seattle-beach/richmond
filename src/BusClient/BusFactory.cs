namespace Richmond.BusClient
{
    public interface IBusFactory
    {
        Bus FromWeatherBus(WeatherBusDatum wbBus, long epochNow);
    }

    public class BusFactory : IBusFactory
    {
        public Bus FromWeatherBus(WeatherBusDatum wbBus, long epochNow)
        {
            var hasPrediction = wbBus.PredictedTime != 0;
            var deltaMilliseconds = (hasPrediction ? wbBus.PredictedTime : wbBus.ScheduledTime) - epochNow;

            var status = "scheduled";
            if (hasPrediction)
            {
                status = "on-time";
                if (wbBus.PredictedTime > wbBus.ScheduledTime)
                {
                    status = "late";
                }
                else if (wbBus.PredictedTime < wbBus.ScheduledTime)
                {
                    status = "early";
                }
            }

            return new Bus
            {
                ShortName = wbBus.RouteShortName,
                LongName = wbBus.Headsign,
                Eta = deltaMilliseconds / (60 * 1000),
                Status = status
            };
        }
    }
}