using System;
using System.Collections.Generic;
using System.IO;
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

            var streak = new Store.Streak(@"c:\temp\streaks\abc");

            Task.Factory.StartNew(() =>
            {
                var es = new List<Event>(1000);

                for (int j = 0; j < 1000000; j++)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        es.Add(new Event
                        {
                            Type = "Test.Event",
                            Data = $" Tick: {i}",
                            Meta = $" CorrelationId: {i}"
                        });
                    }

                    streak.Save(es);
                    es.Clear();
                }

            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(1000);

            foreach (var e in streak.Get(from: 1, to: 1000000000, continuous: true))
            {
                if (e.Position % 100000 == 0) Console.WriteLine($"{DateTime.UtcNow.TimeOfDay} Got {e.Position}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
