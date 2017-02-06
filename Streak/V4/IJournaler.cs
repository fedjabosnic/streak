using Streak.V2.OS;

namespace Streak.V4
{
    public interface IJournaler : ICommittable
    {
        SegmentInfo Append(byte[] data);
    }
}