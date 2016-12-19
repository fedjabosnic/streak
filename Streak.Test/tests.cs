using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streak.Store;

namespace Streak.Test
{
    [TestClass]
    public class tests
    {
        [TestMethod]
        public void should_work()
        {
            true.Should().Be(true);
        }

        // NOTES: Below are some preliminary tests that hit the hard disk (ignored by default)

        [Ignore]
        [TestMethod]
        public void write()
        {
            var index = File.Open(@"c:\temp\streaks\abc\index.ski", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            var events = File.Open(@"c:\temp\streaks\abc\events.ske", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            var streak = new FileStreak(index, events);

            var es = new List<FileStreak.Event>(1);

            var timer = new Stopwatch();

            timer.Start();

            for (int i = 0; i < 1000000; i++)
            {
                es.Add(new FileStreak.Event { Type = "Test.Event", Data = $" Tick: {i}", Meta = $" CorrelationId: {i}" });
                streak.Save(es);
                es.Clear();
            }

            timer.Stop();

            Console.WriteLine(timer.ElapsedMilliseconds);
        }

        [Ignore]
        [TestMethod]
        public void write_batch()
        {
            var index = File.Open(@"c:\temp\streaks\abc\index.ski", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            var events = File.Open(@"c:\temp\streaks\abc\events.ske", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            var streak = new FileStreak(index, events);

            var es = new List<FileStreak.Event>(1000);

            var timer = new Stopwatch();

            timer.Start();

            for (int j = 0; j < 1000; j++)
            {
                for (int i = 0; i < 1000; i++)
                {
                    es.Add(new FileStreak.Event
                    {
                        Type = "Test.Event",
                        Data = $" Tick: {i}",
                        Meta = $" CorrelationId: {i}"
                    });
                }

                streak.Save(es);
                es.Clear();
            }

            timer.Stop();

            Console.WriteLine(timer.ElapsedMilliseconds);
        }

        [Ignore]
        [TestMethod]
        public void write_bulk()
        {
            var index = File.Open(@"c:\temp\streaks\abc\index.ski", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            var events = File.Open(@"c:\temp\streaks\abc\events.ske", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            var streak = new FileStreak(index, events);

            var es = new List<FileStreak.Event>();

            for (int i = 0; i < 1000000; i++)
            {
                es.Add(new FileStreak.Event { Type = "Test.Event", Data = $" Tick: {i}", Meta = $" CorrelationId: {i}" });
            }

            var timer = new Stopwatch();

            timer.Start();

            streak.Save(es);

            timer.Stop();

            Console.WriteLine(timer.ElapsedMilliseconds);
        }

        [Ignore]
        [TestMethod]
        public void read()
        {
            var index = File.Open(@"c:\temp\streaks\abc\index.ski", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            var events = File.Open(@"c:\temp\streaks\abc\events.ske", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            var streak = new FileStreak(index, events);

            var timer = new Stopwatch();

            var es = new List<FileStreak.Event>();

            timer.Start();

            foreach (var e in streak.Get())
            {
                es.Add(e);
            }

            timer.Stop();

            Console.WriteLine(timer.ElapsedMilliseconds);
        }

        [Ignore]
        [TestMethod]
        public void read_write()
        {
            Task.Factory.StartNew(() =>
            {
                var w_index = File.Open(@"c:\temp\streaks\abc\index.ski", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                var w_events = File.Open(@"c:\temp\streaks\abc\events.ske", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

                var w_streak = new FileStreak(w_index, w_events);

                for (int i = 0; i < 1000000000; i++)
                {
                    Thread.Sleep(10);

                    w_streak.Save(new List<FileStreak.Event>
                    {
                        new FileStreak.Event
                        {
                            Type = "Test.Event",
                            Data = $" Tick: {i}",
                            Meta = $" CorrelationId: {i}"
                        }
                    });
                }
            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(3000);

            var r_index = File.Open(@"c:\temp\streaks\abc\index.ski", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var r_events = File.Open(@"c:\temp\streaks\abc\events.ske", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var r_streak = new FileStreak(r_index, r_events);

            foreach (var e in r_streak.Get(from: 100, to: 200, continuous: true))
            {
                if (e.Position % 100000 == 0) Debug.WriteLine($"{DateTime.UtcNow.TimeOfDay} Got {e.Position}");
            }

        }
    }
}
