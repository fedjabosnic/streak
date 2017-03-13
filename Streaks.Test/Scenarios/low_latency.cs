using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streaks.Core;

namespace Streaks.Test.Scenarios
{
    [TestClass]
    public class low_latency
    {
        private static int count = 1000000;
        private static string path = $@"{Environment.CurrentDirectory}\low_latency";

        private IStreakReader reader;
        private IStreakWriter writer;

        private List<long> writes = new List<long>(count*3);
        private List<long> reads = new List<long>(count*3);

        private Dictionary<long, long> timings = new Dictionary<long, long>();

        private List<byte[]> data = new List<byte[]>(count*3);

        [TestInitialize]
        public void Setup()
        {
            // JIT warm up

            var streak = Streak.Open(path + @"\jit");

            using (var w = streak.Writer())
            using (var r = streak.Reader())
            {
                w.Write(new byte[10]);
                w.Write(new byte[10]);
                w.Write(new byte[10]);
                w.Discard();

                w.Write(new byte[10]);
                w.Commit();

                w.Write(new byte[10]);
                w.Write(new byte[10]);
                w.Commit();

                var a = r.Read(1);
                var b = r.Read(2);
                var c = r.Read(3);

                Console.WriteLine(a + "" +  b + "" + c);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            reader.Dispose();
            writer.Dispose();

            Directory.Delete(path, true);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var streak = Streak.Open(path);

            writer = streak.Writer();
            reader = streak.Reader();

            var clock = streak.Advanced().Clock;
            var item = new byte[10];

            var gc0 = GC.CollectionCount(0);
            var gc1 = GC.CollectionCount(1);
            var gc2 = GC.CollectionCount(2);

            var timer = new Stopwatch();
            timer.Start();

            var readerTask = Task.Factory.StartNew(() =>
            {
                for (var i = 1; i <= count; i++)
                {
                    //data.Add(reader.Read(i));
                    timings[i] = clock.Time.Ticks - BitConverter.ToInt64(reader.Read(i), 0);

                    reads.Add(clock.Time.Ticks);
                }

            }, TaskCreationOptions.LongRunning);

            var writerTask = Task.Factory.StartNew(() =>
            {
                for (var i = 1; i <= count; i++)
                {
                    writer.Write(BitConverter.GetBytes(clock.Time.Ticks));

                    writes.Add(clock.Time.Ticks);

                    if (i % 1 == 0) writer.Commit();
                }

            }, TaskCreationOptions.LongRunning);

            Task.WaitAll(writerTask, readerTask);

            gc0 = GC.CollectionCount(0) - gc0;
            gc1 = GC.CollectionCount(1) - gc1;
            gc2 = GC.CollectionCount(2) - gc2;

            timer.Stop();

            var deltas = Enumerable.Zip(writes, reads, (w, r) => (r - w) / 10).ToList();
            //var deltas = timings.Select(x => x.Value / 10).ToList();

            Console.WriteLine($"Count: {count}");
            Console.WriteLine();
            Console.WriteLine($"Throughput : {$"{count / ((double)timer.ElapsedTicks / Stopwatch.Frequency):N}",14} /s");
            Console.WriteLine();
            Console.WriteLine($"Latency max: {$"{deltas.Max():N}",14} micros");
            Console.WriteLine($"        avg: {$"{deltas.Average():N}",14} micros");
            Console.WriteLine($"        min: {$"{deltas.Min():N}",14} micros");
            Console.WriteLine();
            Console.WriteLine($"        99%: {$"{Percentile(deltas, 0.99):N}",14} micros");
            Console.WriteLine($"        90%: {$"{Percentile(deltas, 0.90):N}",14} micros");
            Console.WriteLine($"        50%: {$"{Percentile(deltas, 0.50):N}",14} micros");
            Console.WriteLine();
            Console.WriteLine($"        std: {$"{StdDev(deltas):N}",14} micros");
            Console.WriteLine();
            Console.WriteLine($"Garbage col: ");
            Console.WriteLine($"         G0: {gc0}");
            Console.WriteLine($"         G1: {gc1}");
            Console.WriteLine($"         G2: {gc2}");

            Console.WriteLine(data.Count);
            Console.WriteLine(writerTask.Id);
            Console.WriteLine(readerTask.Id);
        }

        public long Percentile(List<long> sequence, double excelPercentile)
        {
            var sorted = sequence.OrderBy(x => x).ToList();

            var N = sorted.Count();
            var n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1d) return sorted[0];
            else if (n == N) return sorted[N - 1];
            else
            {
                var k = (int)n;
                var d = n - k;
                return (long)(sorted[k - 1] + d * (sorted[k] - sorted[k - 1]));
            }
        }

        public double StdDev(List<long> values)
        {
            var ret = (double)0;
            var count = values.Count();

            if (count > 1)
            {
                // Compute the Average
                var avg = values.Average();

                // Perform the Sum of (value-avg)^2
                var sum = values.Sum(d => (d - avg) * (d - avg));

                // Put it all together
                ret = Math.Sqrt(sum / count);
            }

            return ret;
        }
    }
}
