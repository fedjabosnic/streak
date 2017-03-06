using System.IO;

namespace Streaks.Core.IO
{
    public class FileWriter : IFileWriter
    {
        internal FileStream File { get; }

        public long Length { get; private set; }
        public long Position { get; private set; }

        public FileWriter(string path)
        {
            // NOTE: File handling and buffering
            // - The file stream is opened with a small buffer size which effectively disables FileStream's internal buffering.
            // - The file stream is opened with exclusive write permissions, therefore we are safe to manage the position/length internally.

            File = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, 1, FileOptions.SequentialScan);

            Length = File.Length;
            Position = File.Position;
        }

        public void Write(byte[] data, int offset, int length)
        {
            File.Write(data, offset, length);

            Position += length;
            Length += length;
        }

        public void Flush(bool sync = false)
        {
            File.Flush(sync);
        }

        public void Discard()
        {
        }

        public void Dispose()
        {
            File?.Dispose();
        }
    }
}