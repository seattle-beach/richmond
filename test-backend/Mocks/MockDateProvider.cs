using System;

namespace Richmond.Tests
{
    public class MockDateProvider : IDateProvider
    {
        private readonly DateTime _now;
        private readonly long _epochNow;

        public MockDateProvider(DateTime now)
        {
            _now = now;
        }

        public MockDateProvider(long epochNow)
        {
            _epochNow = epochNow;
        }

        public DateTime Now()
        {
            return _now;
        }

        public long EpochNow()
        {
            return _epochNow;
        }
    }
}