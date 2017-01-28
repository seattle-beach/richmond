using System;

namespace Richmond
{
    public interface IDateProvider
    {
        DateTime Now();
        ulong EpochNow();
    }

    public class DateProvider : IDateProvider
    {
        public DateTime Now()
        {
            DateTime utcNow = DateTime.UtcNow;
            var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            return TimeZoneInfo.ConvertTime(utcNow, pacificTimeZone);
        }

        public ulong EpochNow()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime utcNow = DateTime.UtcNow;
            return (ulong)Math.Floor((utcNow - origin).TotalMilliseconds);
        }
    }
}