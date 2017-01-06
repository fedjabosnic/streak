using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streak.Core;

namespace Streak.Test
{
    [TestClass]
    public class performance_tests
    {
        // NOTE: Below are some preliminary performance tests that hit the hard disk (ignored by default so shouldn't auto-run)
        // NOTE: They are left here for convenience and will be removed later...

        [Ignore]
        [TestMethod]
        public void write()
        {
            var streak = new global::Streak.Core.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            var es = new List<Event>(1);

            var timer = new Stopwatch();

            timer.Start();

            for (int i = 0; i < 1000000; i++)
            {
                es.Add(new Event { Type = "Test.Event", Data = $" Tick: {i}", Meta = $" CorrelationId: {i}" });
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
            var streak = new global::Streak.Core.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            var es = new List<Event>(1000);

            var timer = new Stopwatch();

            timer.Start();

            for (int j = 0; j < 1000; j++)
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

            timer.Stop();

            Console.WriteLine(timer.ElapsedMilliseconds);
        }

        [Ignore]
        [TestMethod]
        public void write_bulk()
        {
            var streak = new global::Streak.Core.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            var es = new List<Event>();

            for (int i = 0; i < 1000000; i++)
            {
                es.Add(new Event { Type = "Test.Event", Data = $" Tick: {i}", Meta = $" CorrelationId: {i}" });
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
            var streak = new global::Streak.Core.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            var timer = new Stopwatch();

            var es = new List<Event>();

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
            var streak = new global::Streak.Core.Streak($@"{Environment.CurrentDirectory}\abc", writer: true);

            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 1000000000; i++)
                {
                    Thread.Sleep(10);

                    streak.Save(new List<Event>
                    {
                        new Event
                        {
                            Type = "Test.Event",
                            Data = $" Tick: {i}",
                            Meta = $" CorrelationId: {i}"
                        }
                    });
                }
            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(3000);

            foreach (var e in streak.Get(from: 100, to: 200, continuous: true))
            {
                if (e.Position % 100000 == 0) Debug.WriteLine($"{DateTime.UtcNow.TimeOfDay} Got {e.Position}");
            }

        }
    }
}
