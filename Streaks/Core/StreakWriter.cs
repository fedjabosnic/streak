using System;
using Streaks.Core.Data;
using Streaks.Core.IO;
using Streaks.Utilities;

namespace Streaks.Core
{
    public class StreakWriter : IStreakWriter
    {
        internal IClock Clock { get; }
        internal IFileWriter Log { get; }
        internal IFileWriter Index { get; }

        internal StreakWriter(IClock clock, IFileWriter log, IFileWriter index)
        {
            Clock = clock;

            Log = log;
            Index = index;
        }

        public void Write(byte[] data)
        {
            var log = new LogEntry { Data = data };
            var index = new IndexEntry { Offset = Log.Position, Length = data.Length };

            Log.Write(log);
            Index.Write(index);
        }

        public void Commit()
        {
            Log.Flush();
            Index.Flush();
        }

        public void Discard()
        {
            Log.Discard();
            Index.Discard();
        }

        public void Dispose()
        {
            Log?.Dispose();
            Index?.Dispose();
        }
    }
}