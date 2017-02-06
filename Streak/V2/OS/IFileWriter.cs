namespace Streak.V2.OS
{
    public interface IFileWriter : IFile
    {
        SegmentInfo Write(byte[] data);
        void Flush();
    }
}