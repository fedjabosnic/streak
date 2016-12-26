using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Streak.Store;

namespace Streak.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //var abc = new Store.Streak(@"c:\temp\streaks\abc", false);
            //var def = new Store.Streak(@"c:\temp\streaks\def", false);

            //var abcs = abc.Get(1, 10, false).ToList();
            //var defs = def.Get(1, 10, false).ToList();



            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            var streak = new Store.Streak(@"c:\temp\streaks\abc", writer: true);

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

                    streak.Save(es);
                    es.Clear();
                }

            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(1000);

            var streak2 = new Store.Streak(@"c:\temp\streaks\def", writer: true);

            Task.Factory.StartNew(() =>
            {
                var batch = 1000;
                
                var es2 = new List<Event>(batch);

                foreach (var e in streak.Get(from: streak2.Length + 1, to: 100000000, continuous: true))
                {
                    es2.Add(e);

                    if (e.Position % batch == 0)
                    {
                        streak2.Save(es2);
                        es2.Clear();
                    }
                }
            }, TaskCreationOptions.LongRunning);

            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"{DateTime.UtcNow.TimeOfDay:g}: {streak.Length}/{streak2.Length}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
