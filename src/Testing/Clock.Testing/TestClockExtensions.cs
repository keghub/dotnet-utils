using System;

namespace EMG.Utilities
{
    public static class TestClockExtensions
    {
        public static void ResetToCurrent(this TestClock clock)
        {
            clock.SetTo(DateTimeOffset.UtcNow);
        }
    }
}