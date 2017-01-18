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
            return DateTime.Now;
        }
    }
}