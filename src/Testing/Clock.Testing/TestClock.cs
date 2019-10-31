using System;

namespace EMG.Utilities
{
    public class TestClock : IClock
    {
        private DateTimeOffset _utcNow;

        public TestClock(DateTimeOffset initialTime)
        {
            UtcNow = initialTime;
        }

        public DateTimeOffset UtcNow
        {
            get => _utcNow;
            set => _utcNow = value.ToUniversalTime();
        }

        public void AdvanceBy(TimeSpan interval)
        {
            UtcNow += interval;
        }

        public void SetTo(DateTimeOffset newValue)
        {
            UtcNow = newValue;
        }
    }
}