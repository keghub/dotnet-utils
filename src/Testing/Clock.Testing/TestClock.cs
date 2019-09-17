using System;

namespace EMG.Utilities
{
    public class TestClock : IClock
    {
        public TestClock(DateTimeOffset initialTime)
        {
            UtcNow = initialTime;
        }

        public DateTimeOffset UtcNow { get; set; }

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