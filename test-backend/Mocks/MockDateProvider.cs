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

        public DateTime Now()
        {
            return _now;
        }

        public ulong EpochNow()
        {
            return 0;
        }
    }
}