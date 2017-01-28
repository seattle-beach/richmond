using Richmond.BusClient;
using System.Linq;
using Xunit;

namespace Richmond.Tests
{
    public class BusScheduleClientTests
    {
        [Fact]
        public void ClientCanFetch()
        {
            var mockHttpClient = new MockHttpClientWrapper();
            var subject = new BusScheduleClient(mockHttpClient, null);
            var buses = subject.Fetch();
            Assert.NotEmpty(buses.Data);

            var bus = buses.Data.First();
            Assert.Equal("99", bus.RouteShortName);
            Assert.Equal("Sand Point East Green Lake", bus.Headsign);
            Assert.Equal(1485476239000, bus.PredictedTime);
            Assert.Equal(1485476239000, bus.ScheduledTime);
        }
    }
}