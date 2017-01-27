using Xunit;
using System.Linq;

namespace Richmond.Tests
{
    public class BusScheduleClientTests
    {
        [Fact]
        public void ClientCanFetch()
        {
            var mockHttpClient = new MockHttpClientWrapper();
            var subject = new BusScheduleClient(mockHttpClient);
            var buses = subject.Fetch();
            Assert.NotEmpty(buses);
            Assert.Equal("99", buses.First().ShortName);
        }
    }
}