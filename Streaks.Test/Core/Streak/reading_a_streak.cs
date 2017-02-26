using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streaks.Core;

namespace Streaks.Test.Core.Streak
{
    [TestClass]
    public class reading_a_streak
    {
        private IStreak reader;
        private IStreak writer;

        private List<Entry> output;

        [TestInitialize]
        public void Setup()
        {
            writer = new global::Streaks.Streak(Environment.CurrentDirectory, writer: true);

            var input = new List<Entry>
            {
                new Entry { Data = "test data 1" },
                new Entry { Data = "test data 2" },
                new Entry { Data = "test data 3" }
            };

            writer.Save(input);

            reader = new global::Streaks.Streak(Environment.CurrentDirectory);

            output = reader.Get().ToList();
        }

        [TestCleanup]
        public void Cleanup()
        {
            reader.Dispose();
            writer.Dispose();

            File.Delete(Environment.CurrentDirectory + @"\main.ind");
            File.Delete(Environment.CurrentDirectory + @"\main.dat");
        }

        [TestMethod]
        public void should_return_the_correct_number_of_events()
        {
            output.Should().HaveCount(3);
        }

        [TestMethod]
        public void should_return_the_correct_position_for_each_event()
        {
            output[0].Position.Should().Be(1);
            output[1].Position.Should().Be(2);
            output[2].Position.Should().Be(3);
        }

        [TestMethod]
        public void should_return_the_correct_timestamp_for_each_event()
        {
            // TODO: Add time abstraction
        }

        [TestMethod]
        public void should_return_the_correct_data_for_each_event()
        {
            output[0].Data.Should().Be("test data 1");
            output[1].Data.Should().Be("test data 2");
            output[2].Data.Should().Be("test data 3");
        }
    }
}