using System.IO;

namespace Streak.V2.OS
{

    public interface IFileReader : IFile
    {
        SegmentInfo Read(byte[] data, int cycle = 1);
        long Seek(long offset, SeekOrigin origin);
    }
}