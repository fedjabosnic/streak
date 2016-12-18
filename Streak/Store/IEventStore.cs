using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Streak.Store
{
    public interface IStreak<T>
    {
        /// <summary>
        /// Saves the specified events into the stream. The operation is pesimistically concurrent, where a failure to insert any
        /// single event results in all events failing to save.
        /// </summary>
        /// <param name="events">The events to save.</param>
        void Save(IEnumerable<T> events);

        /// <summary>
        /// Gets a live enumerable of events between the 'from' and 'to' positions. Optionally, if 'continuous' is specified
        /// as true, the enumerable will block until more data is available (waits for further inserts up to the 'to' position).
        /// </summary>
        /// <param name="from">The position to get events from.</param>
        /// <param name="to">The position to get events to.</param>
        /// <param name="continuous">Whether this is a continuous query.</param>
        /// <returns>A live enumerable of events.</returns>
        IEnumerable<T> Get(long from = 0, long to = long.MaxValue, bool continuous = false);
    }

    public class FileStreak : IStreak<FileStreak.Event>
    {
        private readonly Stream _index;
        private readonly Stream _events;

        public long Length => _index.Length >= 16 ? _index.Length / 16 : 0;

        public FileStreak(Stream index, Stream events)
        {
            _index = index;
            _events = events;
        }

        public void Save(IEnumerable<Event> events)
        {
            // Positions index:
            // position | from | to
            // 1        | 0    | 128
            // 2        | 129  | 523
            // 3        | 524  | 785

            // TODO: Validate events being saved and exit early

            var position = Length;

            var starts = _events.Length;
            var ends = _events.Length;

            _index.Position = _index.Length;
            _events.Position = _events.Length;

            foreach (var e in events)
            {
                e.Position = ++position;
                e.Timestamp = DateTime.UtcNow;

                var raw = e.Serialize();

                ends += raw.Length;

                // Write index
                _index.Write(BitConverter.GetBytes(starts), 0, 8);
                _index.Write(BitConverter.GetBytes(ends), 0, 8);

                // Write event
                _events.Write(raw, 0, raw.Length);

                _index.Flush();
                _events.Flush();

                starts = ends;
            }
        }

        public IEnumerable<Event> Get(long @from = 0, long to = long.MaxValue, bool continuous = false)
        {
            var temp = new byte[8];

            var max = to > Length ? Length : to;

            var starts = 0L;
            var ends = 0L;

            if (max > from)
            {
                var lengths = new int[max - from];

                _index.Position = from*16;
                _index.Read(temp, 0, 8);
                _events.Position = BitConverter.ToInt64(temp, 0);
                _index.Position = from*16;

                for (var i = from; i < max; i++)
                {
                    _index.Read(temp, 0, 8);
                    starts = BitConverter.ToInt64(temp, 0);
                    _index.Read(temp, 0, 8);
                    ends = BitConverter.ToInt64(temp, 0);

                    lengths[i - from] = (int) (ends - starts);
                }

                foreach (var length in lengths)
                {
                    var raw = new byte[length];

                    _events.Read(raw, 0, length);

                    var e = new Event();
                    e.Deserialize(raw);

                    yield return e;
                }
            }

            if (continuous)
            {
                for (var i = from; i < to; i++)
                {
                    while (i >= Length) Thread.Sleep(10);

                    _index.Position = i * 16;

                    _index.Read(temp, 0, 8);
                    starts = BitConverter.ToInt64(temp, 0);

                    _index.Read(temp, 0, 8);
                    ends = BitConverter.ToInt64(temp, 0);

                    var raw = new byte[ends - starts];

                    _events.Position = starts;
                    _events.Read(raw, 0, raw.Length);

                    var e = new Event();
                    e.Deserialize(raw);

                    yield return e;
                }
            }
        }

        public class Event
        {
            /// <summary> The position. </summary>
            public long? Position { get; set; }
            /// <summary> The timestamp. </summary>
            public DateTime? Timestamp { get; set; }

            /// <summary> The event type. </summary>
            public string Type { get; set; }
            /// <summary> The event data. </summary>
            public string Data { get; set; }
            /// <summary> The event meta data. </summary>
            public string Meta { get; set; }

            public byte[] Serialize()
            {
                if (Position == null) throw new Exception();
                if (Timestamp == null) throw new Exception();

                using (var stream = new MemoryStream())
                using (var sw = new BinaryWriter(stream))
                {
                    sw.Write(Position.Value);
                    sw.Write(Timestamp.Value.Ticks);
                    sw.Write(Type);
                    sw.Write(Data);
                    sw.Write(Meta);

                    return stream.GetBuffer();
                }
            }

            public void Deserialize(byte[] source)
            {
                using (var stream = new MemoryStream(source))
                using (var sw = new BinaryReader(stream))
                {
                    Position = sw.ReadInt64();
                    Timestamp = new DateTime(sw.ReadInt64(), DateTimeKind.Utc);
                    Type = sw.ReadString();
                    Data = sw.ReadString();
                    Meta = sw.ReadString();
                }
            }
        }
    }
}
