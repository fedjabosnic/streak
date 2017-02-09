using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streak.V2.OS;
using Streak.V4;

namespace Streak.Test
{
    [TestClass]
    public class performance_tests_v4
    {
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

        //[Ignore]
        [TestMethod]
        public void write_raw()
        {
            var indexer = new Indexer(new FileWriter($@"{Environment.CurrentDirectory}\abc\xxx.ind", 5600000));
            var journaler = new Journaler(new FileWriter($@"{Environment.CurrentDirectory}\abc\xxx.dat", 5600000));
            var committer = new Committer(int.MaxValue, int.MaxValue);
            var streak = new V4.Streak(indexer, journaler, committer);

            var entries = new List<byte[]>(count);

            for (var i = 0; i < count; i++)
            {
                entries.Add(encoding.GetBytes($"fdfsadfsfdsadhfsghdjkafgkjgshdfjkgsh: {i:D10}\n"));
            }

            var timer = new Stopwatch();

            timer.Start();

            foreach (var entry in entries)
            {
                var position = streak.Append(new Entry { Data = entry });
                //if (position % 1000 == 0) streak.Commit();
            }

            //streak.Commit();

            timer.Stop();

            Thread.Sleep(1000);

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }

        //[Ignore]
        [TestMethod]
        public void write_raw_blaa()
        {
            var index = new WritableFile($@"{Environment.CurrentDirectory}\abc\00000.index");
            var journal = new WritableFile($@"{Environment.CurrentDirectory}\abc\00000.journal");
            var committer = new DoubleBufferedStreamCommitter(index, journal);
            var appender = new Appender(committer);

            var entries = new List<Entry>(count);

            for (var i = 1; i <= count; i++)
            {
                entries.Add(new Entry { Data = encoding.GetBytes($"{RandomString(36)}: {i:D10}\n") });
            }

            var timer = new Stopwatch();

            timer.Start();

            //var j = 0;

            foreach (var entry in entries)
            {
                //j++;
                appender.Append(entry);
                //appender.Commit();
                //var position = streak.Append(new Entry { Data = entry });
                //if (position % 1000 == 0) streak.Commit();
            }

            Console.WriteLine("---- DONE ----");
            //streak.Commit();

            timer.Stop();

            Thread.Sleep(1000);

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }
    }
}
