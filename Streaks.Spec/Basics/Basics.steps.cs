using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Streaks.Core;
using TechTalk.SpecFlow;

namespace Streaks.Test.Specifications
{
    [Binding]
    public class BasicsSteps
    {
        private static readonly string Path = $@"{Environment.CurrentDirectory}\Basics";

        private IStreak streak;
        private IStreakReader reader;
        private IStreakWriter writer;

        private byte[][] results;

        [BeforeScenario]
        public void Setup()
        {
            streak = Streak.Open(Path);

            writer = streak.Writer();
            reader = streak.Reader();

            results = new byte[4][];
        }

        [AfterScenario]
        public void Cleanup()
        {
            reader.Dispose();
            writer.Dispose();

            Directory.Delete(Path, true);
        }

        [Given(@"an empty streak")]
        public void given_an_empty_streak()
        {
        }

        [Given(@"a streak with ""(.*)"" entries")]
        public void given_a_streak_with_some_entries(int entries)
        {
            for (var i = 1; i <= entries; i++)
            {
                writer.Write(new [] { (byte)i });
            }

            writer.Commit();
        }

        [Given(@"a writer is already open")]
        public void given_a_writer_is_already_open()
        {
        }

        [Given(@"a reader is already open")]
        public void given_a_reader_is_already_open()
        {
        }

        [When(@"I open another writer")]
        public void when_i_open_another_writer()
        {
            try
            {
                using (streak.Writer());
            }
            catch (Exception e)
            {
                ScenarioContext.Current["Error"] = e;
            }
        }

        [When(@"I open another reader")]
        public void when_i_open_another_reader()
        {
            try
            {
                using (streak.Reader());
            }
            catch (Exception e)
            {
                ScenarioContext.Current["Error"] = e;
            }
        }

        [When(@"I write ""(.*)"" bytes of data")]
        public void when_i_write_some_bytes_of_data(int bytes)
        {
            writer.Write(new byte[bytes]);
        }

        [When(@"I do not commit")]
        public void when_i_do_not_commit()
        {
        }

        [When(@"I commit")]
        public void when_i_commit()
        {
            writer.Commit();
        }

        [When(@"I discard")]
        public void when_i_discard()
        {
            writer.Discard();
        }

        [When(@"I read the entries sequentially")]
        public void when_i_read_the_entries_sequentially()
        {
            results[0] = reader.Read(1);
            results[1] = reader.Read(2);
            results[2] = reader.Read(3);
            results[3] = reader.Read(4);
        }

        [When(@"I read the entries in reverse")]
        public void when_i_read_the_entries_in_reverse()
        {
            results[3] = reader.Read(4);
            results[2] = reader.Read(3);
            results[1] = reader.Read(2);
            results[0] = reader.Read(1);
        }

        [When(@"I read the entries randomly")]
        public void when_i_read_the_entries_randomly()
        {
            results[2] = reader.Read(3);
            results[0] = reader.Read(1);
            results[1] = reader.Read(2);
            results[3] = reader.Read(4);
        }

        [When(@"I try to read entry ""(.*)""")]
        public void when_i_try_to_read_entry(int entry)
        {
            try
            {
                reader.Read(entry);
            }
            catch (Exception e)
            {
                ScenarioContext.Current["Error"] = e;
            }
        }

        [Then(@"the reader should not see any data")]
        public void the_reader_should_not_see_any_data()
        {
            reader.Count.Should().Be(0);
        }

        [Then(@"the reader should see ""(.*)"" entries totalling ""(.*)"" bytes")]
        public void the_reader_should_see_some_entries_totalling_some_bytes(int entries, int bytes)
        {
            reader.Count.Should().Be(entries);

            var total = 0;

            for (var i = 1; i <= entries; i++)
            {
                total += reader.Read(i).Length;
            }

            total.Should().Be(bytes);
        }

        [Then(@"I should read the correct data")]
        public void i_should_read_the_correct_data()
        {
            results[0][0].Should().Be(1);
            results[1][0].Should().Be(2);
            results[2][0].Should().Be(3);
            results[3][0].Should().Be(4);
        }

        [Then(@"there should be no error")]
        public void then_there_should_be_no_error()
        {
            ScenarioContext.Current.Should().NotContainKey("Error");
        }

        [Then(@"there should be an error")]
        public void then_there_should_be_an_error()
        {
            ScenarioContext.Current.Should().ContainKey("Error");
        }

        [Then(@"it should say ""(.*)""")]
        public void then_it_should_say(string message)
        {
            ScenarioContext.Current.Should().ContainKey("Error");
            ScenarioContext.Current["Error"].As<Exception>().Message.Should().Be(message);
        }
    }
}
