using Richmond.BusClient;
using Xunit;

namespace Richmond.Tests
{
    public class BusFactoryTests
    {
        private const long epochNow = 1485476239000;
        private WeatherBusDatum GenerateWeatherBus()
        {
            return new WeatherBusDatum
            {
                RouteShortName = "99",
                Headsign = "Sand Point East Green Lake",
                PredictedTime = epochNow + 2 * 60 * 1000,
                ScheduledTime = epochNow + 4 * 60 * 1000
            };
        }

        [Fact]
        public void WeatherbusToBus()
        {
            var subject = new BusFactory();

            var wbBus = GenerateWeatherBus();

            var bus = subject.FromWeatherBus(wbBus, epochNow);
            Assert.Equal("99", bus.ShortName);
            Assert.Equal("Sand Point East Green Lake", bus.LongName);
            Assert.Equal(2, bus.Eta);
            Assert.Equal("early", bus.Status);
        }

        [Fact]
        public void MissedBus()
        {
            var subject = new BusFactory();

            var wbBus = GenerateWeatherBus();
            wbBus.PredictedTime = epochNow - 2 * 60 * 1000;

            var bus = subject.FromWeatherBus(wbBus, epochNow);
            Assert.Equal(-2, bus.Eta);
        }

        [Fact]
        public void NoPredictedTime()
        {
            var subject = new BusFactory();

            var wbBus = GenerateWeatherBus();
            wbBus.PredictedTime = 0;

            var bus = subject.FromWeatherBus(wbBus, epochNow);
            Assert.Equal(4, bus.Eta);
            Assert.Equal("scheduled", bus.Status);
        }

        [Fact]
        public void LateBus()
        {
            var subject = new BusFactory();

            var wbBus = GenerateWeatherBus();
            wbBus.PredictedTime = wbBus.ScheduledTime + 5 * 60 * 1000;

            var bus = subject.FromWeatherBus(wbBus, epochNow);
            Assert.Equal("late", bus.Status);
        }

        [Fact]
        public void OnTimeBus()
        {
            var subject = new BusFactory();

            var wbBus = GenerateWeatherBus();
            wbBus.PredictedTime = wbBus.ScheduledTime;

            var bus = subject.FromWeatherBus(wbBus, epochNow);
            Assert.Equal("on-time", bus.Status);
        }
    }
}