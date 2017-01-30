using System;

namespace Richmond
{
    public interface IDateProvider
    {
        DateTime Now();
        long EpochNow();
    }

    public class DateProvider : IDateProvider
    {
        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public long EpochNow()
        {
            DateTime utcNow = DateTime.UtcNow;
            var delta = utcNow - epoch;
            return (long)Math.Floor(delta.TotalMilliseconds);
        }

        public DateTime Now()
        {
            DateTime utcNow = DateTime.UtcNow;
            var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            return TimeZoneInfo.ConvertTime(utcNow, pacificTimeZone);
        }
    }
}