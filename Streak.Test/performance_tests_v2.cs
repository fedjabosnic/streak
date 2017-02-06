using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streak.V3.Journal;

namespace Streak.Test
{
    [TestClass]
    public class performance_tests_v2
    {
        private readonly int count = 1000000;
        private readonly Encoding encoding = new UTF8Encoding();

        // NOTE: Below are some preliminary performance tests that hit the hard disk (ignored by default so shouldn't auto-run)
        // NOTE: They are left here for convenience and will be removed later...

        [Ignore]
        [TestMethod]
        public void file_rename_test()
        {
            var file = new FileInfo($@"{Environment.CurrentDirectory}\abc\main.000");
            var stream = new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read | FileShare.Delete, 512, FileOptions.SequentialScan);

            //var buffer = new byte[512];

            var timer = new Stopwatch();

            timer.Start();

            for (int i = 1; i <= count; i++)
            {
                //var f = $@"{Environment.CurrentDirectory}\abc\main.{i:D10}";

                //file.MoveTo(f);
                //file.
                //File.AppendAllText(f, $"testing {i}");

                //Thread.Sleep(1000);

                stream.Seek(0, SeekOrigin.Begin);

                var bytes = encoding.GetBytes($"testing {i}");
                stream.Write(bytes, 0, bytes.Length);

                stream.Flush();
                //File.Move(file, $@"{Environment.CurrentDirectory}\test_{i}.txt");
            }

            timer.Stop();

            Console.WriteLine($"Entries: {count}");
            Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"         {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }

        [Ignore]
        [TestMethod]
        public void file_flush_test()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var stream = new FileStream($@"{Environment.CurrentDirectory}\test.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 512, FileOptions.SequentialScan);

                    var buffer = new byte[512];

                    buffer[0] = 123;
                    buffer[10] = 124;

                    stream.Lock(0, 50);

                    Thread.Sleep(1000);

                    for (int i = 1; i <= 10; i++)
                    {
                        Console.WriteLine($"Writing 1-{i}");
                        stream.Write(buffer, 0, 10);
                        Thread.Sleep(50);
                    }

                    Console.WriteLine("Finished 1");

                    stream.Unlock(0, 50);

                    //stream.Flush();

                    Console.WriteLine("Flushed 1");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var stream = new FileStream($@"{Environment.CurrentDirectory}\test.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 512, FileOptions.SequentialScan);

                    var buffer = new byte[512];

                    buffer[0] = 177;
                    buffer[10] = 178;

                    stream.Lock(50, 100);

                    Thread.Sleep(1000);

                    stream.Position = 50;

                    for (int i = 1; i <= 10; i++)
                    {
                        Console.WriteLine($"Writing 2-{i}");
                        stream.Write(buffer, 0, 10);
                        Thread.Sleep(100);
                    }

                    Console.WriteLine("Finished 2");

                    stream.Unlock(50, 100);

                    //stream.Flush();

                    Console.WriteLine("Flushed 2");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(3000);
        }

        [Ignore]
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

        [Ignore]
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

        [Ignore]
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

        [Ignore]
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

        [Ignore]
        [TestMethod]
        public void read_write()
        {
            var writer = new V2.Writer.Streak($@"{Environment.CurrentDirectory}\abc");

            Thread.Sleep(100);

            Task.Factory.StartNew(() =>
            {
                //var writer = new V2.Writer.Streak($@"{Environment.CurrentDirectory}\abc");

                var entries2 = new List<byte[]>();

                for (var i = 1; i <= count; i++)
                {
                    entries2.Add(encoding.GetBytes($"fdfsadfsfdsadhfsghdjkafgkjgshdfjkgsd: {i:D10}\n"));
                }

                var timer2 = new Stopwatch();

                timer2.Start();

                for (var i = 1; i <= entries2.Count; i++)
                {
                    var entry = entries2[i-1];
                    Console.WriteLine("Adding entry " + i);
                    writer.Append(new V2.Writer.Streak.Entry {Data = entry});
                    Thread.Sleep(1);
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

            for (int i = 1; i <= count; i++)
            {
                entries.Add(reader.Next().Data);
                Console.WriteLine("Received entry " + i + ": " + encoding.GetString(entries[i-1]));
            }

            timer.Stop();

            Console.WriteLine($"Reader Entries: {count}");
            Console.WriteLine($"Reader Elapsed: {timer.ElapsedMilliseconds} millis");
            Console.WriteLine($"Reader Rates:   {count / ((double)timer.ElapsedTicks / Stopwatch.Frequency)} entries per second");
            Console.WriteLine($"                {((double)timer.ElapsedTicks / Stopwatch.Frequency) / count * 1000000} micros per entry");
        }
    }
}
