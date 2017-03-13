using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Streaks.Core;

namespace Streaks.Dsl
{
    public static class Replication
    {
        public static IStreak ReplicateTo(this IStreak source, IStreak destination, int batch = 1)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        // TODO: Need to be able to get length from destination
                        //destination.Save(source.Get(destination.Length + 1, long.MaxValue), 1);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Replication failed (retrying in a second): {ex}");

                        Thread.Sleep(1000);
                    }
                }
            },
            TaskCreationOptions.LongRunning);

            return source;
        }
    }
}
