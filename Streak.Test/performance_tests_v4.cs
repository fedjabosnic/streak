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

        [Ignore]
        [TestMethod]
        public void write_raw()
        {
            var indexer = new Indexer(new FileWriter($@"{Environment.CurrentDirectory}\abc\xxx.ind"));
            var journaler = new Journaler(new FileWriter($@"{Environment.CurrentDirectory}\abc\xxx.dat"));
            var committer = new Committer(100, 1);
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
                streak.Append(new Entry { Data = entry });
            }

            timer.Stop();

            Thread.Sleep(2000);

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }
    }
}
