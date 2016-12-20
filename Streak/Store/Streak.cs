using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Streak.Store
{
    public class Streak : IStreak<Event>
    {
        private readonly Stream _index;
        private readonly Stream _events;

        public long Length => _index.Length >= 16 ? _index.Length / 16 : 0;

        public Streak(Stream index, Stream events)
        {
            _index = index;
            _events = events;
        }

        public void Save(IEnumerable<Event> events)
        {
            // TODO: Add error handling

            var position = Length;

            var starts = _events.Length;
            var ends = _events.Length;

            _index.Position = _index.Length;
            _events.Position = _events.Length;

            foreach (var e in events)
            {
                e.Position = ++position;
                e.Timestamp = DateTime.UtcNow;

                // Write data
                ends += e.SerializeTo(_events);

                // Write index (improve this)
                _index.Write(BitConverter.GetBytes(starts), 0, 8);
                _index.Write(BitConverter.GetBytes(ends), 0, 8);

                _events.Flush();
                _index.Flush();

                starts = ends;
            }
        }

        public IEnumerable<Event> Get(long @from = 1, long to = long.MaxValue, bool continuous = false)
        {
            // If data is not yet available, either wait or exit
            if (from > Length)
            {
                if (continuous)
                    while (from > Length) Thread.Sleep(10);
                else
                    yield break;
            }

            var exists = to > Length ? Length : to;

            var temp = new byte[8];
            _index.Position = from * 16 - 16;
            _index.Read(temp, 0, 8);
            _events.Position = BitConverter.ToInt64(temp, 0);

            // Get any currently available data
            for (var i = from; i <= exists; i++)
            {
                var e = new Event();

                e.DeserializeFrom(_events);

                yield return e;
            }

            // Wait for any upcoming data
            if (continuous && to > exists)
            {
                for (var i = exists + 1; i <= to; i++)
                {
                    while (i > Length) Thread.Sleep(10);

                    var e = new Event();

                    e.DeserializeFrom(_events);

                    yield return e;
                }
            }
        }
    }
}