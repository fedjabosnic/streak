using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streak.Store;

namespace Streak.Test.Store.Streak
{
    [TestClass]
    public class reading_a_streak_subset
    {
        private List<Event> output;

        [TestInitialize]
        public void Setup()
        {
            var index = new MemoryStream();
            var events = new MemoryStream();

            var writer = new global::Streak.Store.Streak(index, events);

            var input = new List<Event>
            {
                new Event { Type = "Test.Event.A", Data = "Tick: 1", Meta = "CorrelationId: 1" },
                new Event { Type = "Test.Event.B", Data = "Tick: 2", Meta = "CorrelationId: 2" },
                new Event { Type = "Test.Event.C", Data = "Tick: 3", Meta = "CorrelationId: 3" },
                new Event { Type = "Test.Event.D", Data = "Tick: 4", Meta = "CorrelationId: 4" },
                new Event { Type = "Test.Event.E", Data = "Tick: 5", Meta = "CorrelationId: 5" }
            };

            writer.Save(input);

            var reader = new global::Streak.Store.Streak(index, events);

            output = reader.Get(from: 2, to: 4).ToList();
        }

        [TestMethod]
        public void should_return_the_correct_number_of_events()
        {
            output.Should().HaveCount(3);
        }

        [TestMethod]
        public void should_return_the_correct_position_for_each_event()
        {
            output[0].Position.Should().Be(2);
            output[1].Position.Should().Be(3);
            output[2].Position.Should().Be(4);
        }

        [TestMethod]
        public void should_return_the_correct_timestamp_for_each_event()
        {
            // TODO: Add time abstraction
        }

        [TestMethod]
        public void should_return_the_correct_type_for_each_event()
        {
            output[0].Type.Should().Be("Test.Event.B");
            output[1].Type.Should().Be("Test.Event.C");
            output[2].Type.Should().Be("Test.Event.D");
        }

        [TestMethod]
        public void should_return_the_correct_data_for_each_event()
        {
            output[0].Data.Should().Be("Tick: 2");
            output[1].Data.Should().Be("Tick: 3");
            output[2].Data.Should().Be("Tick: 4");
        }

        [TestMethod]
        public void should_return_the_correct_meta_for_each_event()
        {
            output[0].Meta.Should().Be("CorrelationId: 2");
            output[1].Meta.Should().Be("CorrelationId: 3");
            output[2].Meta.Should().Be("CorrelationId: 4");
        }
    }
}
