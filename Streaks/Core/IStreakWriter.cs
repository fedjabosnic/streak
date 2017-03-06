using System;

namespace Streaks.Core
{
    public interface IStreakWriter : IDisposable
    {
        void Write(byte[] data);
        void Commit();
        void Discard();
    }
}