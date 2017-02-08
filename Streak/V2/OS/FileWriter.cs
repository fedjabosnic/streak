using System.IO;

namespace Streak.V2.OS
{
    /// <summary>
    /// Allows writing to a file using a dynamically resizing buffer.
    /// <remarks>
    /// The buffer is resized to accomodate all data within a flush period, therefore the buffer will grow based on flush frequency.
    /// </remarks>
    /// /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    /// </summary>
    public class FileWriter : IFileWriter
    {
        private readonly FileStream _stream;
        private readonly Buffer _buffer;

        public string Name => _stream.Name;
        public long Length => _stream.Length;
        public long Position { get; private set; }

        /// <summary>
        /// Opens a file at the specified path for writing.
        /// <remarks>
        /// The file will be opened in append-only mode with its position set to the end of the file - it is not possible to write at random locations.
        /// </remarks>
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <param name="buffer">The initial buffer capacity.</param>
        public FileWriter(string path, int buffer = 4096)
        {
            // NOTE: Filestream gets a buffer size of 1 to simply prevent it's internal buffering as we are buffering manually
            _stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, 1, FileOptions.SequentialScan);
            _buffer = new Buffer(buffer);
        }

        /// <summary>
        /// Writes the bytes from the provided buffer to the file.
        /// <remarks>
        /// All writes are copied and internally buffered so the data will not be written until the writer is flushed.
        /// </remarks>
        /// </summary>
        /// <param name="buffer">The data to write.</param>
        /// <returns>File segment information describing where in the file the data will be written once flushed.</returns>
        public FileSegmentInfo Write(byte[] buffer)
        {
            _buffer.Add(buffer);

            return new FileSegmentInfo { Offset = Position += buffer.Length, Length = buffer.Length };
        }

        /// <summary>
        /// Flushes the data to the file.
        /// </summary>
        public void Flush()
        {
            _stream.Write(_buffer.Data, 0, _buffer.Length);
            _stream.Flush();

            _buffer.Clear();
        }

        /// <summary>
        /// Disposes the file writer along with the underlying file handle.
        /// <remarks>
        /// Unflushed changes will not be flushed to the file.
        /// </remarks>
        /// </summary>
        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}