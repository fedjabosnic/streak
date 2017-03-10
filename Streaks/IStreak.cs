using Streaks.Core;
using Streaks.Utilities;

namespace Streaks
{
    public interface IStreak
    {
        IAdvancedStreak Advanced();

        IStreakReader Reader();
        IStreakWriter Writer();
    }

    public interface IAdvancedStreak : IStreak
    {
        IClock Clock { get; set; }
    }
}