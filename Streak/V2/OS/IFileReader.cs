using System.IO;

namespace Streak.V2.OS
{
    /// <summary>
    /// Allows reading from a file.
    /// </summary>
    public interface IFileReader : IFile
    {
        /// <summary>
        /// Seeks to the specified location in the file.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The origin for the seek.</param>
        /// <returns>The new position in the file.</returns>
        long Seek(long offset, SeekOrigin origin);

        /// <summary>
        /// Reads data into the provided byte array.
        /// <remarks>
        /// The intention is for the call to block until there is enough data to fill the buffer fully.
        /// </remarks>
        /// </summary>
        /// <param name="data">The buffer to read into.</param>
        /// <param name="cycle"></param>
        /// <returns>File segment information describing where in the file the data was read from.</returns>
        FileSegmentInfo Read(byte[] data, int cycle = 1);
    }
}