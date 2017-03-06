using Streaks.Core;

namespace Streaks
{
    public interface IStreak
    {
        IStreakReader Reader();
        IStreakWriter Writer();
    }
}