using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
            //Console.WriteLine("Press any key to create data...");
            //Console.ReadKey();


            // ------------------------------------------------------------

            //var data = new List<V2.Reader.Streak.Entry>(1000000);
            //var streak = new V2.Reader.Streak($@"{Environment.CurrentDirectory}\aaa");

            //Console.WriteLine("Press any key read data...");
            //Console.ReadKey();

            //for (int i = 0; i < 1000000; i++)
            //{
            //    data.Add(streak.Append());
            //}

            ////Console.WriteLine("Done...");
            //Console.ReadKey();

            //return;

            // ------------------------------------------------------------

            //var data = new List<V2.Writer.Streak.Entry>();

            //for (int i = 0; i < 1000000; i++)
            //{
            //    data.Add(new V2.Writer.Streak.Entry
            //    {
            //        Data = Encoding.UTF8.GetBytes($"fsdfsadfsfdsadhfsghdjkafgkjgshdfjkgsdfkjhgasdjkfgsajdfgasjdhfgjasdghfjsagdfjkgasdfjgsdj: {++i:D10}")
            //    });
            //}

            //var streak = new V2.Writer.Streak($@"{Environment.CurrentDirectory}\aaa");

            //Console.WriteLine("Press any key save data...");
            //Console.ReadKey();

            //foreach (var d in data) streak.Append(d);

            ////Console.WriteLine("Done...");
            //Console.ReadKey();

            //return;

            // ------------------------------------------------------------

            Console.WriteLine($"{DateTime.UtcNow.TimeOfDay:g}: Starting");

            var _ = new UTF8Encoding();

            var written = 0L;
            var read = 0L;

            var times = new List<long>();

            Task.Factory.StartNew(() =>
            {
                var writer = new V2.Writer.Streak($@"{Environment.CurrentDirectory}\aaa");

                for (int i = 0; i < 100000000; i++)
                {
                    var z = new byte[100];
                    var time = BitConverter.GetBytes(HighResolutionDateTime.UtcNow.Ticks);
                    time.CopyTo(z, 0);

                    writer.Append(new V2.Writer.Streak.Entry { Data = z });
                    written++;
                }

            }, TaskCreationOptions.LongRunning);


            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);

                var reader = new V2.Reader.Streak($@"{Environment.CurrentDirectory}\aaa");

                for (int i = 0; i < 1000000000; i++)
                {
                    var now = HighResolutionDateTime.UtcNow;

                    var bytes = reader.Next().Data;
                    var then = new DateTime(BitConverter.ToInt64(bytes, 0), DateTimeKind.Utc);

                    times.Add(now.Ticks - then.Ticks);

                    read++;
                }

            }, TaskCreationOptions.LongRunning);


            while (true)
            {
                var ts = Interlocked.Exchange(ref times, new List<long>());
                var ws = Interlocked.Exchange(ref written, 0L);
                var rs = Interlocked.Exchange(ref read, 0L);

                // Some weirdness causes huge deltas even though they aren't true
                ts.RemoveAll(x => x > 10000000);

                Console.Clear();
                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss.fff}:> Entries (w/r): {ws:D10} <-> {rs:D10}");
                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss.fff}:> Latency max:   {(ts.Count == 0 ? 0 : ts.Max(x => x / 10))} micros");
                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss.fff}:>         avg:   {(ts.Count == 0 ? 0 : ts.Average(x => x / 10))} micros");
                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss.fff}:>         min:   {(ts.Count == 0 ? 0 : ts.Min(x => x / 10))} micros");
                Thread.Sleep(1000);
            }



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

    public static class HighResolutionDateTime
    {
        public static bool IsAvailable { get; private set; }
        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        public static DateTime UtcNow
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException("High resolution clock isn't available.");
                }
                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);
                return DateTime.FromFileTimeUtc(filetime);
            }
        }
        static HighResolutionDateTime()
        {
            try
            {
                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);
                IsAvailable = true;
            }
            catch (EntryPointNotFoundException)
            {
                // Not running Windows 8 or higher.
                IsAvailable = false;
            }
        }
    }
}