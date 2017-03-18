using System;

namespace Streaks.Core.IO
{
    public interface IFileWriter : IDisposable
    {
        long Length { get; }
        long Position { get; }

        void Write(byte[] entry);
        void Flush(bool force = false);
        void Discard();
    }
}