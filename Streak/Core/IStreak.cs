using System;
using System.Collections.Generic;

namespace Streak.Core
{
    public interface IStreak : IDisposable
    {
        /// <summary> Returns the number of events in the streak. </summary>
        long Length { get; }

        /// <summary>
        /// Saves the specified events into the stream. The operation is pesimistically concurrent, where a failure to insert any
        /// single event results in all events failing to save.
        /// </summary>
        /// <param name="events">The events to save.</param>
        void Save(IEnumerable<Entry> events);

        /// <summary>
        /// Gets a live enumerable of events between the 'from' and 'to' positions (inclusive). Optionally, if 'continuous' is specified
        /// as true, the enumerable will block until more data is available (waits for further inserts up to the 'to' position).
        /// </summary>
        /// <param name="from">The position to get events from.</param>
        /// <param name="to">The position to get events to.</param>
        /// <param name="continuous">Whether this is a continuous query.</param>
        /// <returns>A live enumerable of events.</returns>
        IEnumerable<Entry> Get(long from = 1, long to = long.MaxValue, bool continuous = false);
    }
}