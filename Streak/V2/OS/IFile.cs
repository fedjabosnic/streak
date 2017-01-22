using System;
using System.IO;

namespace Streak.V2.OS
{
    public interface IFile : IDisposable
    {
        long Length { get; }

        long Seek(long offset, SeekOrigin origin);
        int Read(byte[] array, int offset, int count);
        void Write(byte[] array, int offset, int count);
        void Flush();
    }

    public class File : IFile
    {
        private readonly FileStream _file;

        public long Length => _file.Length;

        public File(string path, FileMode mode, FileAccess access, FileShare share, int buffersize, FileOptions options)
        {
            _file = new FileStream(path, mode, access, share, buffersize, options);
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return _file.Seek(offset, origin);
        }

        public int Read(byte[] array, int offset, int count)
        {
            return _file.Read(array, offset, count);
        }

        public void Write(byte[] array, int offset, int count)
        {
            _file.Write(array, offset, count);
        }

        public void Flush()
        {
            _file.Flush();
        }

        public void Dispose()
        {
            _file?.Dispose();
        }
    }
}
