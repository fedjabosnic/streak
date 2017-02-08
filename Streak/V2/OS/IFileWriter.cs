namespace Streak.V2.OS
{
    /// <summary>
    /// Allows writing to a file.
    /// <remarks>
    /// The flushing semantics are decided by implementing classes, data may or may not be written to disk prior to flushing.
    /// </remarks>
    /// </summary>
    public interface IFileWriter : IFile
    {
        /// <summary>
        /// Writes the bytes from the provided buffer to the file.
        /// <remarks>
        /// The intention is that when the call returns you are allowed to manipulate the buffer again.
        /// </remarks>
        /// </summary>
        /// <param name="buffer">The data to write.</param>
        /// <returns>File segment information describing where in the file the data was written.</returns>
        FileSegmentInfo Write(byte[] buffer);

        /// <summary>
        /// Flushes any unflushed data to the underlying file.
        /// </summary>
        void Flush();
    }
}