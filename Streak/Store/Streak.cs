using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Streak.Store
{
    public class Streak : IStreak<Event>
    {
        private readonly string _index;
        private readonly string _data;

        public long Length => new FileInfo(_index).Length / 16;

        public Streak(string path)
        {
            _index = $@"{path}\index.ski";
            _data = $@"{path}\data.ske";

            if (!File.Exists(_index)) File.Create(_index).Dispose();
            if (!File.Exists(_data)) File.Create(_data).Dispose();
        }

        public void Save(IEnumerable<Event> events)
        {
            // TODO: Add error handling

            using (var index = File.Open(_index, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            using (var data = File.Open(_data, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                var position = index.Length / 16;

                var starts = data.Length;
                var ends = data.Length;

                index.Position = index.Length;
                data.Position = data.Length;

                foreach (var e in events)
                {
                    e.Position = ++position;
                    e.Timestamp = DateTime.UtcNow;

                    // Write data
                    ends += e.SerializeTo(data);

                    // Write index (improve this)
                    index.Write(BitConverter.GetBytes(starts), 0, 8);
                    index.Write(BitConverter.GetBytes(ends), 0, 8);

                    data.Flush(true);
                    index.Flush(true);

                    starts = ends;
                }
            }
        }

        public IEnumerable<Event> Get(long @from = 1, long to = long.MaxValue, bool continuous = false)
        {
            using (var index = File.Open(_index, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var data = File.Open(_data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // If data is not yet available, either wait or exit
                if (from > index.Length / 16)
                {
                    if (continuous)
                        while (from > index.Length / 16) Thread.Sleep(10);
                    else
                        yield break;
                }

                var exists = to > index.Length / 16 ? index.Length / 16 : to;

                var temp = new byte[8];
                index.Position = from * 16 - 16;
                index.Read(temp, 0, 8);
                data.Position = BitConverter.ToInt64(temp, 0);

                // Get any currently available data
                for (var i = from; i <= exists; i++)
                {
                    var e = new Event();

                    e.DeserializeFrom(data);

                    yield return e;
                }

                // Wait for any upcoming data
                if (continuous && to > exists)
                {
                    for (var i = exists + 1; i <= to; i++)
                    {
                        while (i > index.Length / 16) Thread.Sleep(10);

                        var e = new Event();

                        e.DeserializeFrom(data);

                        yield return e;
                    }
                }
            }
        }
    }
}