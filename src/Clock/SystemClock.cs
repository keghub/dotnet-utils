using System;

namespace EMG.Utilities
{
    public class SystemClock : IClock
    {
        private SystemClock()
        {
            
        }

        public static readonly IClock Instance = new SystemClock();

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}