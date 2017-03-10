using System;
using System.IO;
using Streaks.Core;
using Streaks.Core.IO;
using Streaks.Utilities;

namespace Streaks
{
    public class Streak : IAdvancedStreak
    {
        public string Path { get; }
        public IClock Clock { get; set; }

        public static IStreak Open(string path)
        {
            return new Streak(path) { Clock = new Clock() };
        }

        internal Streak(string path)
        {
            Path = path;
        }

        public IAdvancedStreak Advanced()
        {
            return this;
        }

        public IStreakReader Reader()
        {
            if (!Directory.Exists(Path)) throw new Exception();

            var clock = Clock;

            var log = new FileReader($@"{Path}\000000000000001.log", 512000);
            var index = new FileReader($@"{Path}\000000000000001.index", 512000);

            return new StreakReader(clock, log, index);
        }

        public IStreakWriter Writer()
        {
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);

            var clock = Clock;

            var log = new FileWriter($@"{Path}\000000000000001.log", 512000);
            var index = new FileWriter($@"{Path}\000000000000001.index", 512000);

            return new StreakWriter(clock, log, index);
        }
    }
}