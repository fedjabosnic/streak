using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streaks.Core;

namespace Streaks.Test.Core.Streak
{
    [TestClass]
    public class reading_a_streak
    {
        private string path = $@"{Environment.CurrentDirectory}\reading_a_streak";

        private IStreakReader reader;
        private IStreakWriter writer;

        private List<byte[]> output = new List<byte[]>();

        [TestInitialize]
        public void Setup()
        {
            var streak = Streaks.Streak.Open(path);

            writer = streak.Writer();

            writer.Write(Encoding.UTF8.GetBytes("aaa"));
            writer.Write(Encoding.UTF8.GetBytes("bbb"));
            writer.Write(Encoding.UTF8.GetBytes("ccc"));

            reader = streak.Reader();

            output.Add(reader.Read(1));
            output.Add(reader.Read(2));
            output.Add(reader.Read(3));
        }

        [TestCleanup]
        public void Cleanup()
        {
            reader.Dispose();
            writer.Dispose();

            Directory.Delete(path, true);
        }

        [TestMethod]
        public void should_return_the_correct_number_of_events()
        {
            output.Should().HaveCount(3);
        }

        [TestMethod]
        public void should_return_the_correct_position_for_each_event()
        {
            // TODO
        }

        [TestMethod]
        public void should_return_the_correct_timestamp_for_each_event()
        {
            // TODO
        }

        [TestMethod]
        public void should_return_the_correct_data_for_each_event()
        {
            Encoding.UTF8.GetString(output[0]).Should().Be("aaa");
            Encoding.UTF8.GetString(output[1]).Should().Be("bbb");
            Encoding.UTF8.GetString(output[2]).Should().Be("ccc");
        }
    }
}