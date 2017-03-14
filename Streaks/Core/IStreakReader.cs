using System;

namespace Streaks.Core
{
    public interface IStreakReader : IDisposable
    {
        long Count { get; }

        byte[] Read(long position);
    }
}