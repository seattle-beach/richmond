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

            var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");

            int actualDifference = (utcNow.Day > pacificNow.Day ? 24 : 0) + utcNow.Hour - pacificNow.Hour;
            // DateTime.IsDaylightSavingsTime() is broken and checks against system local timezone not the timezone in the DateTime itself
            int expectedDifference = pacificTimeZone.IsDaylightSavingTime(pacificNow) ? 7 : 8;
            
            Assert.Equal(expectedDifference, actualDifference);
        }
    }
}
