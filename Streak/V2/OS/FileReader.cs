using System.IO;

namespace Streak.V2.OS
{
    /// <summary>
    /// Allows reading from a file using a fixed-size buffer.
    /// /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    /// </summary>
    public class FileReader : IFileReader
    {
        private readonly FileStream _stream;

        public string Name => _stream.Name;
        public long Length => _stream.Length;
        public long Position { get; private set; }

        /// <summary>
        /// Opens a file at the specified path for reading.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <param name="buffer">The initial buffer capacity.</param>
        public FileReader(string path, int buffer = 4096)
        {
            _stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, buffer, FileOptions.SequentialScan);
        }

        /// <summary>
        /// Seeks to the specified location in the file.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The origin for the seek.</param>
        /// <returns>The new position in the file.</returns>
        public long Seek(long offset, SeekOrigin origin)
        {
            return Position = _stream.Seek(offset, origin);
        }

        /// <summary>
        /// Reads data into the provided byte array.
        /// <remarks>
        /// Data will always fill the entire array provided, the call will block until there is more data available.
        /// </remarks>
        /// </summary>
        /// <param name="data">The data buffer to read into.</param>
        /// <param name="cycle"></param>
        /// <returns>File segment information describing where in the file the data was read from.</returns>
        public FileSegmentInfo Read(byte[] data, int cycle = 1)
        {
            var read = 0;

            while ((read = _stream.Read(data, 0 + read, data.Length - read)) != data.Length)
            {
                // TODO: Use a reading strategy to control latency
            }

            return new FileSegmentInfo { Offset = Position + read, Length = read };
        }

        /// <summary>
        /// Disposes the file reader along with the underlying file handle.
        /// </summary>
        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}