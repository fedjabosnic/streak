using System;

namespace Streaks.Core.IO
{
    public interface IFileReader : IDisposable
    {
        long Length { get; }
        long Position { get; }

        void Move(long length);
        void Read(byte[] buffer, int offset, int length);
    }
}