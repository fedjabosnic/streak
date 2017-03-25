using System;

namespace Streaks.Core
{
    public interface IStreakWriter : IDisposable
    {
        long Size { get; }
        long Count { get; }

        void Write(byte[] data);
        void Commit();
        void Discard();
    }
}