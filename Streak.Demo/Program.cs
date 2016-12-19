using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Streak.Store;

namespace Streak.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            Task.Factory.StartNew(() =>
            {
                var w_index = File.Open(@"d:\temp\streaks\abc\index.ski", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                var w_events = File.Open(@"d:\temp\streaks\abc\events.ske", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

                var w_streak = new FileStreak(w_index, w_events);

                var es = new List<FileStreak.Event>(1000);

                for (int j = 0; j < 1000000; j++)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        es.Add(new FileStreak.Event
                        {
                            Type = "Test.Event",
                            Data = $" Tick: {i}",
                            Meta = $" CorrelationId: {i}"
                        });
                    }

                    w_streak.Save(es);
                    es.Clear();
                }

            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(1000);

            var r_index = File.Open(@"d:\temp\streaks\abc\index.ski", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var r_events = File.Open(@"d:\temp\streaks\abc\events.ske", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var r_streak = new FileStreak(r_index, r_events);

            foreach (var e in r_streak.Get(from: 1, to: 1000000000, continuous: true))
            {
                if (e.Position % 100000 == 0) Console.WriteLine($"{DateTime.UtcNow.TimeOfDay} Got {e.Position}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
