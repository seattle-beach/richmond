using LightMock;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Richmond.Tests
{
    public class BusScheduleControllerTests
    {
        [Fact]
        public void ReturnsResponse()
        {
            var mockBusServiceContext = new MockContext<IBusService>();
            var busServiceMock = new BusServiceMock(mockBusServiceContext);
            var mockDateProviderContext = new MockContext<IDateProvider>();
            var dateProviderMock = new DateProviderMock(mockDateProviderContext);
            
            ulong epochNow = 1485392200000;
            var busServiceData = new BusServiceResponse
            {
                Data = new List<BusServiceResponse.Bus>()
                {
                    new BusServiceResponse.Bus {RouteShortName = "99", Headsign = "Belltown Via 1st Ave", PredictedTime = epochNow + (2 * 60 * 1000), ScheduledTime = epochNow + (4 * 60 * 1000)}
                }
            };
            mockBusServiceContext.Arrange(f => f.BusesForStop()).Returns(busServiceData);
            mockDateProviderContext.Arrange(f => f.EpochNow()).Returns(epochNow);

            var subject = new BusScheduleController(busServiceMock, dateProviderMock);

            var result = subject.Buses();

            mockBusServiceContext.Assert(f => f.BusesForStop());
            mockDateProviderContext.Assert(f => f.EpochNow());

            var response = result as OkObjectResult;
            Assert.NotNull(response);

            var payload = response.Value as BusResponse;
            Assert.NotNull(payload);

            Assert.Equal(1, payload.Buses.Count());

            var bus = payload.Buses.First();
            Assert.Equal("99", bus.ShortName);
            Assert.Equal("Belltown Via 1st Ave", bus.LongName);
            Assert.Equal(2, bus.Eta);
            Assert.Equal("early", bus.Status);
        }

        public class BusServiceMock : IBusService
        {
            private readonly IInvocationContext<IBusService> context;

            public BusServiceMock(IInvocationContext<IBusService> context)
            {
                this.context = context;
            }

            public BusServiceResponse BusesForStop() {
                return context.Invoke(f => f.BusesForStop());
            }
        }

        public class DateProviderMock : IDateProvider
        {
            private readonly IInvocationContext<IDateProvider> context;

            public DateProviderMock(IInvocationContext<IDateProvider> context)
            {
                this.context = context;
            }

            public ulong EpochNow() {
                return this.context.Invoke(f => f.EpochNow());
            }

            public DateTime Now() {
                throw new NotImplementedException();
            }
        }
    }
}