using Streak.V2.OS;

namespace Streak.V4
{
    public interface IJournaler : ICommittable
    {
        FileSegmentInfo Append(byte[] data);
    }
}