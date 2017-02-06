using System.IO;

namespace Streak.V2.OS
{
    /// <summary>
    /// Allows reading from a file using a fixed-size buffer.
    /// </summary>
    public class FileReader : IFileReader
    {
        private readonly FileStream _stream;

        public string Name => _stream.Name;
        public long Length => _stream.Length;
        public long Position { get; private set; }

        public FileReader(string path, int buffer = 4096)
        {
            _stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, buffer, FileOptions.SequentialScan);
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return Position = _stream.Seek(offset, origin);
        }

        public SegmentInfo Read(byte[] data, int cycle = 1)
        {
            var read = 0;

            while ((read = _stream.Read(data, 0 + read, data.Length - read)) != data.Length)
            {
                // TODO: Use a reading strategy to control latency
            }

            return new SegmentInfo { Offset = Position + read, Length = read };
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }

    public struct SegmentInfo
    {
        public static long Size => 16;

        public long Offset { get; set; }
        public long Length { get; set; }

        public unsafe byte[] ToBytes()
        {
            var data = new byte[Size];

            fixed (byte* b = data) *(long*)(b + 0) = Offset;
            fixed (byte* b = data) *(long*)(b + 8) = Length;

            return data;
        }
    }
}