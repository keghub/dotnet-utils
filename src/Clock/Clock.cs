using System;

namespace EMG.Utilities
{
    public static class Clock
    {
        public static IClock Default { get; private set; } = SystemClock.Instance;

        public static void Set(IClock clock)
        {
            Default = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public static void Reset()
        {
            Default = SystemClock.Instance;
        }
    }
}
