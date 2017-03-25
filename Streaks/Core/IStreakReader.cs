using System;

namespace Streaks.Core
{
    public interface IStreakReader : IDisposable
    {
        long Size { get; }
        long Count { get; }

        byte[] Read(long position);
    }
}