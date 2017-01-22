using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Streak.Test
{
    [TestClass]
    public class performance_tests_v2
    {
        private readonly int count = 10000000;
        private readonly Encoding encoding = new UTF8Encoding();

        // NOTE: Below are some preliminary performance tests that hit the hard disk (ignored by default so shouldn't auto-run)
        // NOTE: They are left here for convenience and will be removed later...

        //[Ignore]
        [TestMethod]
        public void write_raw()
        {
            var streak = new V2.Writer.Streak($@"{Environment.CurrentDirectory}\abc");

            var entries = new List<byte[]>(count);

            for (var i = 0; i < count; i++)
            {
                entries.Add(encoding.GetBytes($"fdfsadfsfdsadhfsghdjkafgkjgshdfjkgsh: {i:D10}\n"));
            }

            var timer = new Stopwatch();

            timer.Start();

            foreach (var entry in entries)
            {
                streak.Append(new V2.Writer.Streak.Entry { Data = entry });
            }

            timer.Stop();

            Thread.Sleep(2000);

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }

        [TestMethod]
        public void write_string()
        {
            var streak = new V2.Writer.Streak($@"{Environment.CurrentDirectory}\abc");

            var entries = new List<string>(count);

            for (var i = 0; i < count; i++)
            {
                entries.Add($"fdfsadfsfdsadhfsghdjkafgkjgshdfjkgsd: {i:D10}\n");
            }

            var timer = new Stopwatch();

            timer.Start();

            foreach (var entry in entries)
            {
                streak.Append(new V2.Writer.Streak.Entry { Data = encoding.GetBytes(entry) });
            }

            timer.Stop();

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }

        //[Ignore]
        [TestMethod]
        public void read_raw()
        {
            var streak = new V2.Reader.Streak($@"{Environment.CurrentDirectory}\abc");

            var entries = new List<byte[]>(count);

            var timer = new Stopwatch();

            timer.Start();

            for (var i = 0; i < count; i++)
            {
                entries.Add(streak.Next().Data);
            }

            timer.Stop();

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }

        [TestMethod]
        public void read_string()
        {
            var streak = new V2.Reader.Streak($@"{Environment.CurrentDirectory}\abc");

            var entries = new List<string>();

            var timer = new Stopwatch();

            timer.Start();

            for (var i = 0; i < count; i++)
            {
                entries.Add(encoding.GetString(streak.Next().Data));
            }

            timer.Stop();

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }

        //[Ignore]
        [TestMethod]
        public void read_write()
        {
            Task.Factory.StartNew(() =>
            {
                var writer = new V2.Writer.Streak($@"{Environment.CurrentDirectory}\abc");

                var entries2 = new List<byte[]>();

                for (var i = 1; i <= count; i++)
                {
                    entries2.Add(encoding.GetBytes($"fdfsadfsfdsadhfsghdjkafgkjgshdfjkgsd: {i:D10}\n"));
                }

                var timer2 = new Stopwatch();

                timer2.Start();

                foreach (var entry in entries2)
                {
                    writer.Append(new V2.Writer.Streak.Entry { Data = entry });
                }

                timer2.Stop();

                Console.WriteLine($"Writer Entries: {count}");
                Console.WriteLine($"Writer Elapsed: {timer2.ElapsedMilliseconds} millis");
                Console.WriteLine($"Writer Rates:   {count / ((double)timer2.ElapsedTicks / Stopwatch.Frequency)} entries per second");
                Console.WriteLine($"                {((double)timer2.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");

            }, TaskCreationOptions.LongRunning);

            var reader = new V2.Reader.Streak($@"{Environment.CurrentDirectory}\abc");

            var entries = new List<byte[]>(1000000);
            var timer = new Stopwatch();

            timer.Start();

            for (int i = 0; i < count; i++)
            {
                entries.Add(reader.Next().Data);
            }

            timer.Stop();

            Console.WriteLine($"Reader Entries: {count}");
            Console.WriteLine($"Reader Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Reader Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"                {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }
    }
}
