using System;
using System.Net;
using Xunit;

namespace Richmond.Tests
{
    public class DataProviderTests
    {
        private IDateProvider dateProvider = new DateProvider();

        [Fact]
        public void NowUsesPacificTime()
        {
            DateTime utcNow = DateTime.UtcNow;

            var pacificNow = dateProvider.Now();

            int actualDifference = (utcNow.Day > pacificNow.Day ? 24 : 0) + utcNow.Hour - pacificNow.Hour;
            int expectedDifference = pacificNow.IsDaylightSavingTime() ? 7 : 8;
            
            var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            Assert.Equal(TimeZoneInfo.Local, pacificTimeZone);
            Assert.Equal(expectedDifference, actualDifference);
        }
    }
}
