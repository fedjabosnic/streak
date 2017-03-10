using System.IO;

namespace Streaks.Core.IO
{
    public class FileReader : IFileReader
    {
        internal FileStream File { get; }

        public long Length => File.Length;
        public long Position { get; private set; }

        public FileReader(string path, int buffer)
        {
            // NOTE: File handling and buffering
            // - The file stream does a decent job of read buffering so we let it do its thing.
            // - The file stream is a private reference, therefore we are safe to manage the position internally.
            // - The file stream can be written to externally, therefore we are not safe to manage the length internally.

            File = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, buffer, FileOptions.SequentialScan);

            Position = File.Position;
        }

        public void Move(long position)
        {
            Position = File.Seek(position, SeekOrigin.Current);
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            var read = 0;

            // Read from the file until we have the expected amount of data
            while ((read += File.Read(buffer, 0 + read, buffer.Length - read)) != buffer.Length)
            {
                // TODO: Use a reading strategy to control latency
            }

            Position += read;
        }

        public void Dispose()
        {
            File?.Dispose();
        }
    }
}