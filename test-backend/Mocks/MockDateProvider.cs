using System;

namespace Richmond.Tests
{
    public class MockDateProvider : IDateProvider
    {
        private readonly DateTime _now;

        public MockDateProvider(DateTime now)
        {
            _now = now;
        }

        DateTime IDateProvider.Now()
        {
            return _now;
        }
    }
}