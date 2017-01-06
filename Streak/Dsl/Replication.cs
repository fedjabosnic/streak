using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streak.Core;

namespace Streak.Dsl
{
    public static class Replication
    {
        public static IStreak<T> ReplicateTo<T>(this IStreak<T> source, IStreak<T> destination, int chunk = 1000)
        {
            Task.Factory.StartNew(() =>
            {
                var count = 0;
                var batch = new List<T>(chunk);

                foreach (var e in source.Get(from: destination.Length + 1, to: long.MaxValue, continuous: true))
                {
                    batch.Add(e);
                    count++;

                    if (count % chunk == 0)
                    {
                        destination.Save(batch);
                        batch.Clear();
                    }
                }
            },
            TaskCreationOptions.LongRunning);

            return source;
        }
    }
}
