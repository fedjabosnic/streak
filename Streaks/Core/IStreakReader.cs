using System;

namespace Streaks.Core
{
    public interface IStreakReader : IDisposable
    {
        byte[] Read(long position);
    }
}