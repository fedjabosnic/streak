using System;
using System.Collections.Generic;

namespace Streaks.Dsl
{
    public static class Enumerable
    {
        public static IEnumerable<byte[]> Get(this IStreak streak, long from, long to)
        {
            using (var reader = streak.Reader())
            {
                while (from <= to)
                {
                    yield return reader.Read(from++);
                }
            }
        }

        public static void Save(this IStreak streak, IEnumerable<byte[]> data, int batch = int.MaxValue)
        {
            using (var writer = streak.Writer())
            {
                var count = 0;

                foreach (var d in data)
                {
                    writer.Write(d);

                    if(batch % ++count == 0) writer.Commit();
                }

                writer.Commit();
            }
        }
    }
}
