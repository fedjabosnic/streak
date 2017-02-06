using System.IO;

namespace Streak.V2.OS
{
    /// <summary>
    /// Allows writing to a file using a dynamically resizing buffer.
    /// </summary>
    public class FileWriter : IFileWriter
    {
        private readonly FileStream _stream;
        private readonly Buffer _buffer;

        public string Name => _stream.Name;
        public long Length => _stream.Length;
        public long Position { get; private set; }

        public FileWriter(string path, int buffersize = 4096)
        {
            _stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, 1, FileOptions.SequentialScan);
            _buffer = new Buffer(buffersize);
        }

        public SegmentInfo Write(byte[] data)
        {
            _buffer.Add(data);

            return new SegmentInfo { Offset = Position += data.Length, Length = data.Length };
        }

        public void Flush()
        {
            _stream.Write(_buffer.Data, 0, _buffer.Length);
            _stream.Flush();

            _buffer.Clear();
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}