using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Streak.Store
{
    public interface IStore
    {
        IStreak<FileStreak.Event> this[string name] { get; }
    }

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

    public static class Streak
    {
        public static IStreak<FileStreak.Event> Memory(string name)
        {
            var index = new MemoryStream();
            var events = new MemoryStream();

            return new FileStreak(index, events);
        }

        public static IStreak<FileStreak.Event> File(string name)
        {
            var index = System.IO.File.Open($@"c:\temp\streaks\{name}\index.ski", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            var events = System.IO.File.Open($@"c:\temp\streaks\{name}\events.ske", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            return new FileStreak(index, events);
        }
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

        public class Entry
        {
            public long From { get; set; }
            public long To { get; set; }

            public long SerializeTo(Stream stream)
            {
                var start = stream.Position;

                using (var sw = new BinaryWriter(stream, new UTF8Encoding(), true))
                {
                    sw.Write(From);
                    sw.Write(To);
                }

                return stream.Position - start;
            }

            public void DeserializeFrom(Stream stream)
            {
                using (var sw = new BinaryReader(stream, new UTF8Encoding(), true))
                {
                    From = sw.ReadInt64();
                    To = sw.ReadInt64();
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

            public long SerializeTo(Stream stream)
            {
                if (Position == null) throw new Exception();
                if (Timestamp == null) throw new Exception();

                var start = stream.Position;

                using (var sw = new BinaryWriter(stream, new UTF8Encoding(), true))
                {
                    sw.Write(Position.Value);
                    sw.Write(Timestamp.Value.Ticks);
                    sw.Write(Type);
                    sw.Write(Data);
                    sw.Write(Meta);
                }

                return stream.Position - start;
            }

            public void DeserializeFrom(Stream stream)
            {
                using (var sw = new BinaryReader(stream, new UTF8Encoding(), true))
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
