using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Streaks.Test
{
    [TestClass]
    public class performance_tests
    {
        // NOTE: Below are some preliminary performance tests that hit the hard disk (ignored by default so shouldn't auto-run) 
        // NOTE: They are left here for convenience and will be removed later... 

        private readonly int count = 1000000;
        private readonly Encoding encoding = new UTF8Encoding();
        private Random random = new Random();

        private string RandomString(int Size)
        {
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();

            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }

        [Ignore]
        [TestMethod]
        public void write()
        {
            var streak = Streak.Open($@"{Environment.CurrentDirectory}\abc");

            using (var writer = streak.Writer())
            {
                //writer.Write(new byte[100]);
                //writer.Commit();
            }

            using (var writer = streak.Writer())
            {
                //writer.Write(new byte[100]);
                //writer.Commit();
            }

            var timer = new Stopwatch();

            using (var writer = streak.Writer())
            {
                var entries = new List<byte[]>(1000);
                var entry = 0;

                for (var i = 0; i < 1000; i++)
                {
                    entries.Add(encoding.GetBytes($"{i:D8} " + RandomString(89) + "\n"));
                }

                timer.Start();

                for (var i = 0; i < count; i++)
                {
                    entry = entry < 999 ? entry + 1 : 0;
                    writer.Write(entries[entry]);

                    writer.Commit();
                }

                //if (writer.LastCommitted != writer.LastWritten) writer.Commit();
            }

            timer.Stop();

            Thread.Sleep(1000);

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }

        [Ignore]
        [TestMethod]
        public void read()
        {
            var streak = Streak.Open($@"{Environment.CurrentDirectory}\abc");

            using (var reader = streak.Reader())
            {
                Console.WriteLine(encoding.GetString(reader.Read(1)));
            }

            using (var reader = streak.Reader())
            {
                Console.WriteLine(encoding.GetString(reader.Read(4)));
            }

            var entries = new List<byte[]>(1000);
            var timer = new Stopwatch();

            var position = 1;

            using (var reader = streak.Reader())
            {
                timer.Start();

                while (position <= count)
                {
                    entries.Add(reader.Read(position));

                    position = position + 1;
                }
            }

            timer.Stop();

            //Console.WriteLine(encoding.GetString(entries[757]));

            Thread.Sleep(1000);

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }

        //[Ignore]
            //[TestMethod]
            //public void write_batch()
            //{
            //    var streak = new global::Streaks.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            //    var es = new List<Entry>(1000);

            //    var timer = new Stopwatch();

            //    timer.Start();

            //    for (int j = 0; j < 1000; j++)
            //    {
            //        for (int i = 0; i < 1000; i++)
            //        {
            //            es.Add(new Entry { Data = $"fsdfsadfsfdsadhfsghdjkafgkjgshdfjkgsd: {i:D10}" });
            //        }

            //        streak.Save(es);
            //        es.Clear();
            //    }

            //    timer.Stop();

            //    Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds}");
            //    Console.WriteLine($"Rate:    {1000000 / ((double)timer.ElapsedMilliseconds / 1000)} e/s");
            //}

            //[Ignore]
            //[TestMethod]
            //public void write_bulk()
            //{
            //    var streak = new global::Streaks.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            //    var es = new List<Entry>();

            //    for (int i = 0; i < 1000000; i++)
            //    {
            //        es.Add(new Entry { Data = $"fsdfsadfsfdsadhfsghdjkafgkjgshdfjkgsd: {i:D10}" });
            //    }

            //    var timer = new Stopwatch();

            //    timer.Start();

            //    streak.Save(es);

            //    timer.Stop();

            //    Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds}");
            //    Console.WriteLine($"Rate:    {1000000 / ((double)timer.ElapsedMilliseconds / 1000)} e/s");
            //}

            //[Ignore]
            //[TestMethod]
            //public void read()
            //{
            //    var streak = new global::Streaks.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            //    var timer = new Stopwatch();

            //    var es = new List<Entry>();

            //    timer.Start();

            //    foreach (var e in streak.Get())
            //    {
            //        es.Add(e);
            //    }

            //    timer.Stop();

            //    Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds}");
            //    Console.WriteLine($"Rate:    {es.Count / ((double)timer.ElapsedMilliseconds / 1000)} e/s");
            //}

            //[Ignore]
            //[TestMethod]
            //public void read_write()
            //{
            //    var streak = new global::Streaks.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            //    Task.Factory.StartNew(() =>
            //    {
            //        for (int i = 0; i < 1000000000; i++)
            //        {
            //            Thread.Sleep(10);

            //            streak.Save(new List<Entry>
            //            {
            //                new Entry { Data = $"fsdfsadfsfdsadhfsghdjkafgkjgshdfjkgsd: {i:D10}" }
            //            });
            //        }
            //    }, TaskCreationOptions.LongRunning);

            //    Thread.Sleep(3000);

            //    foreach (var e in streak.Get(@from: 100, to: 200, continuous: true))
            //    {
            //        if (e.Position % 100000 == 0) Debug.WriteLine($"{DateTime.UtcNow.TimeOfDay} Got {e.Position}");
            //    }

            //}
        }
}
