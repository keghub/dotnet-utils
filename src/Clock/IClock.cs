using System;

namespace EMG.Utilities
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }
}