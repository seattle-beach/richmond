using System;

namespace Richmond
{
    public interface IDateProvider
    {
        DateTime Now();
    }

    public class DateProvider : IDateProvider
    {
        DateTime IDateProvider.Now()
        {
            DateTime utcNow = DateTime.UtcNow;
            var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            return TimeZoneInfo.ConvertTime(utcNow, pacificTimeZone);
        }
    }
}