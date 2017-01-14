using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Streak.Core;
using Streak.Dsl;

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
            Console.WriteLine("- Write 1,000,000 events directly to one stream");
            Console.WriteLine("- Replicate asynchronously to another stream");
            Console.WriteLine("");

            Console.WriteLine("Press any key to start...");
            Console.WriteLine("");

            Console.ReadKey();

            var original = new Core.Streak($@"{Environment.CurrentDirectory}\aaa", writer: true);
            var replica = new Core.Streak($@"{Environment.CurrentDirectory}\bbb", writer: true);

            original.ReplicateTo(replica);

            Task.Factory.StartNew(() =>
            {
                var es = new List<Entry>(1000);

                var position = original.Length;

                for (int j = 0; j < 10000; j++)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        es.Add(new Entry
                        {
                            // Random (ish) 100 byte data
                            Data = $"fsdfsadfsfdsadhfsghdjkafgkjgshdfjkgsdfkjhgasdjkfgsajdfgasjdhfgjasdghfjsagdfjkgasdfjgsdj: {++position:D10}"
                        });
                    }

                    original.Save(es);
                    es.Clear();
                }

            }, TaskCreationOptions.LongRunning);

            while (replica.Length < 10000000)
            {
                Console.WriteLine($"{DateTime.UtcNow.TimeOfDay:g}: {original.Length} <-> {replica.Length}");
                Thread.Sleep(1000);
            }

            Console.WriteLine("");
            Console.WriteLine("Finished");
            Console.WriteLine("");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}