using Microsoft.AspNetCore.Mvc;
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
            var mockClient = new MockBusScheduleClient();
            mockClient.Add(new Bus { ShortName = "99" });

            var subject = new BusScheduleController(mockClient);
            var result = subject.Buses();

            var response = result as OkObjectResult;
            Assert.NotNull(response);

            var schedule = response.Value as BusScheduleResponse;
            Assert.NotNull(schedule);

            Assert.Equal("99", schedule.Buses.First().ShortName);
        }

        [Fact]
        public void ReturnsErrorOnFetchFailure()
        {
            var fetchException = new Exception("failed to fetch");
            var mockClient = new MockBusScheduleClient { Exception = fetchException };

            var subject = new BusScheduleController(mockClient);
            var result = subject.Buses();

            var response = result as StatusCodeResult;

            Assert.Equal(500, response.StatusCode);
        }
    }
}