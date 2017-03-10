using System;

namespace Streaks.Utilities
{
    public interface IClock
    {
        /// <summary>
        /// The current universal date and time.
        /// </summary>
        DateTime Time { get; }
    }
}
