using Microsoft.AspNetCore.Mvc;
using Richmond.BusClient;
using System;
using System.Linq;
using Xunit;

namespace Richmond.Tests
{
    public class BusScheduleControllerTests
    {
        [Fact]
        public void ReturnsCurrentBusSchedule()
        {
            var epochNow = 1485476239000;

            var mockClient = new MockBusScheduleClient();
            mockClient.Add(new WeatherBusDatum
            {
                RouteShortName = "99",
                Headsign = "Sand Point East Green Lake",
                PredictedTime = epochNow + 2 * 60 * 1000,
                ScheduledTime = epochNow + 4 * 60 * 1000
            });

            var subject = new BusScheduleController(mockClient, new MockDateProvider(epochNow), new BusFactory(), new MockLogger());
            var result = subject.Buses();

            var response = result as OkObjectResult;
            Assert.NotNull(response);

            var schedule = response.Value as BusScheduleResponse;
            Assert.NotNull(schedule);
        }

        [Fact]
        public void ReturnsErrorOnFetchFailure()
        {
            var fetchException = new Exception("failed to fetch");
            var mockClient = new MockBusScheduleClient { Exception = fetchException };

            var subject = new BusScheduleController(mockClient, null, null, new MockLogger());
            var result = subject.Buses();

            var response = result as StatusCodeResult;

            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void FiltersTo99()
        {
            var epochNow = 1485476239000;

            var mockClient = new MockBusScheduleClient();
            mockClient.Add(new WeatherBusDatum
            {
                RouteShortName = "99",
                Headsign = "Sand Point East Green Lake",
                PredictedTime = epochNow + 2 * 60 * 1000,
                ScheduledTime = epochNow + 4 * 60 * 1000
            });
            mockClient.Add(new WeatherBusDatum
            {
                RouteShortName = "62",
                Headsign = "boo",
                PredictedTime = epochNow + 2 * 60 * 1000,
                ScheduledTime = epochNow + 4 * 60 * 1000
            });

            var subject = new BusScheduleController(mockClient, new MockDateProvider(epochNow), new BusFactory(), new MockLogger());
            var result = subject.Buses();

            var response = result as OkObjectResult;
            var schedule = response.Value as BusScheduleResponse;
            Assert.Equal(1, schedule.Buses.Count());
            Assert.Equal("99", schedule.Buses.First().ShortName);
        }

        [Fact]
        public void FiltersToTop2()
        {
            var epochNow = 1485476239000;

            var mockClient = new MockBusScheduleClient();
            mockClient.Add(new WeatherBusDatum
            {
                RouteShortName = "99",
                Headsign = "Sand Point East Green Lake",
                PredictedTime = epochNow - 2 * 60 * 1000,
                ScheduledTime = epochNow
            });
            mockClient.Add(new WeatherBusDatum
            {
                RouteShortName = "99",
                Headsign = "Sand Point East Green Lake",
                PredictedTime = epochNow + 5 * 60 * 1000,
                ScheduledTime = epochNow + 6 * 60 * 1000
            });
            mockClient.Add(new WeatherBusDatum
            {
                RouteShortName = "99",
                Headsign = "Sand Point East Green Lake",
                PredictedTime = epochNow + 17 * 60 * 1000,
                ScheduledTime = epochNow + 18 * 60 * 1000
            });

            var subject = new BusScheduleController(mockClient, new MockDateProvider(epochNow), new BusFactory(), new MockLogger());
            var result = subject.Buses();

            var response = result as OkObjectResult;
            var schedule = response.Value as BusScheduleResponse;
            Assert.Equal(2, schedule.Buses.Count());

            var buses = schedule.Buses.ToList();
            Assert.Equal(-2, buses[0].Eta);
            Assert.Equal(5, buses[1].Eta);
        }
    }
}