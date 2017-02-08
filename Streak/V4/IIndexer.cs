using Streak.V2.OS;

namespace Streak.V4
{
    public interface IIndexer : ICommittable
    {
        Index Append(FileSegmentInfo location);
    }
}