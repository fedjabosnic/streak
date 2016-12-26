using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Streak.Store
{
    public class StreakReader : IDisposable
    {
        private readonly string _path;

        public StreakReader(string path)
        {
            _path = path;

            // Ensure directory exists
            if (!Directory.Exists(path)) throw new Exception();
        }

        public void Dispose()
        {
        }

        public IEnumerable<Event> Read(long @from = 1, long to = long.MaxValue, bool continuous = false)
        {
            using (var index = File.Open($@"{_path}\main.ind", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var datas = File.Open($@"{_path}\main.dat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                datas.Position = BitConverter.ToInt64(temp, 0);

                // Get any currently available data
                for (var i = from; i <= exists; i++)
                {
                    var e = new Event();

                    e.DeserializeFrom(datas);

                    yield return e;
                }

                // Wait for any upcoming data
                if (continuous && to > exists)
                {
                    for (var i = exists + 1; i <= to; i++)
                    {
                        while (i > index.Length / 16) Thread.Sleep(10);

                        var e = new Event();

                        e.DeserializeFrom(datas);

                        yield return e;
                    }
                }
            }
        }
    }
}