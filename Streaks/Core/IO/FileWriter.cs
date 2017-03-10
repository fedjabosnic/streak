using System.IO;
using Streaks.Utilities;

namespace Streaks.Core.IO
{
    public class FileWriter : IFileWriter
    {
        internal FileStream File { get; }
        internal Buffer Buffer { get; set; }

        public long Length { get; private set; }
        public long Position { get; private set; }

        public FileWriter(string path, int buffer)
        {
            // NOTE: File handling and buffering
            // - The file stream doesn't provide dynamically resizing buffers or discarding buffered data so we do it manually.
            // - The file stream is opened with a small buffer size which effectively disables FileStream's internal buffering.
            // - The file stream is opened with exclusive write permissions, therefore we are safe to manage the position and length internally.

            File = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, 1, FileOptions.SequentialScan);
            Buffer = new Buffer(buffer);

            Length = File.Length;
            Position = File.Position;
        }

        public void Write(byte[] data, int offset, int length)
        {
            Buffer.Write(data, offset, length);

            Position += length;
            Length += length;
        }

        public void Flush(bool sync = false)
        {
            File.Write(Buffer.Data, 0, Buffer.Length);
            File.Flush(sync);

            Buffer.Clear();
        }

        public void Discard()
        {
            Position -= Buffer.Length;
            Length -= Buffer.Length;

            Buffer.Clear();
        }

        public void Dispose()
        {
            File?.Dispose();
        }
    }
}