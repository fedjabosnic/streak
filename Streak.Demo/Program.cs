using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Streak.Core;

namespace Streak.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Streak demo");
            Console.WriteLine("-----------");
            Console.WriteLine("This is a demo program to show the performance and usage of streaks.");
            Console.WriteLine("");
            Console.WriteLine("This demo will:");
            Console.WriteLine("- Write directly to one stream");
            Console.WriteLine("- Replicate asynchronously to another stream");
            Console.WriteLine("");

            Console.WriteLine("Press any key to start...");
            Console.WriteLine("");

            Console.ReadKey();

            var original = new Core.Streak($@"{Environment.CurrentDirectory}\aaa", writer: true);

            Task.Factory.StartNew(() =>
            {
                var es = new List<Event>(1000);

                for (int j = 0; j < 10000; j++)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        es.Add(new Event
                        {
                            Type = "Test.Event",
                            Data = $" Tick: {i}",
                            Meta = $" CorrelationId: {i}"
                        });
                    }

                    original.Save(es);
                    es.Clear();
                }

            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(1000);

            var replica = new Core.Streak($@"{Environment.CurrentDirectory}\bbb", writer: true);

            Task.Factory.StartNew(() =>
            {
                var batch = 1000;
                
                var es2 = new List<Event>(batch);

                // Tail original streak and replicate its data
                foreach (var e in original.Get(from: replica.Length + 1, to: 100000000, continuous: true))
                {
                    es2.Add(e);

                    if (e.Position % batch == 0)
                    {
                        replica.Save(es2);
                        es2.Clear();
                    }
                }
            }, TaskCreationOptions.LongRunning);

            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"{DateTime.UtcNow.TimeOfDay:g}: {original.Length}/{replica.Length}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
