using System;

namespace Streaks.Core.IO
{
    public interface IFileReader : IDisposable
    {
        long Length { get; }
        long Position { get; }

        void Read(byte[] buffer, long position);
    }
}